// Copyright ©2015-2016 Copper Mountain Technologies
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR
// ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using CopperMountainTech;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AmplifierStability
{
    public partial class FormMain : Form
    {
        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private enum ComConnectionStateEnum
        {
            INITIALIZED,
            NOT_CONNECTED,
            CONNECTED_VNA_NOT_READY,
            CONNECTED_VNA_READY
        }

        private ComConnectionStateEnum previousComConnectionState = ComConnectionStateEnum.INITIALIZED;
        private ComConnectionStateEnum comConnectionState = ComConnectionStateEnum.NOT_CONNECTED;

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private int selectedChannel = -1;
        private bool traceAbsoluteMeasurementConfigurationError = false;
        private bool traceNonTwoPortConfigurationError = false;
        private bool didAutoConfig = false;

        public FormMain()
        {
            InitializeComponent();

            // --------------------------------------------------------------------------------------------------------

            // set form icon
            Icon = Properties.Resources.app_icon;

            // set form title
            Text = Program.programName;

            // disable resizing the window
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = true;

            // position the plug-in in the lower right corner of the screen
            Rectangle workingArea = Screen.GetWorkingArea(this);
            Location = new Point(workingArea.Right - Size.Width - 130,
                                 workingArea.Bottom - Size.Height - 50);

            // always display on top
            TopMost = true;

            // --------------------------------------------------------------------------------------------------------

            // disable ui
            panelMain.Enabled = false;

            // set version label text
            toolStripStatusLabelVersion.Text = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

            // --------------------------------------------------------------------------------------------------------

            // init stability factor selection radio buttons
            radioButtonK.Checked = true;

            // init user message label
            labelUserMessage.Visible = false;
            labelUserMessage.Text = "";

            // init error flags
            traceAbsoluteMeasurementConfigurationError = false;
            traceNonTwoPortConfigurationError = false;

            // init auto config flag
            didAutoConfig = false;

            // update the channel selection combo box
            updateChanComboBox();

            // --------------------------------------------------------------------------------------------------------

            // start the ready timer
            readyTimer.Interval = 250; // 250 ms interval
            readyTimer.Enabled = true;
            readyTimer.Start();

            // start the update timer
            updateTimer.Interval = 250; // 250 ms interval
            updateTimer.Enabled = true;
            updateTimer.Start();

            // --------------------------------------------------------------------------------------------------------
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //
        // Timers
        //
        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void readyTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // is vna ready?
                if (Program.vna.app.Ready)
                {
                    // yes... vna is ready
                    comConnectionState = ComConnectionStateEnum.CONNECTED_VNA_READY;
                }
                else
                {
                    // no... vna is not ready
                    comConnectionState = ComConnectionStateEnum.CONNECTED_VNA_NOT_READY;
                }
            }
            catch (COMException)
            {
                // com connection has been lost
                comConnectionState = ComConnectionStateEnum.NOT_CONNECTED;
                Application.Exit();
                return;
            }

            if (comConnectionState != previousComConnectionState)
            {
                previousComConnectionState = comConnectionState;

                switch (comConnectionState)
                {
                    default:
                    case ComConnectionStateEnum.NOT_CONNECTED:

                        // update vna info text box
                        toolStripStatusLabelVnaInfo.ForeColor = Color.White;
                        toolStripStatusLabelVnaInfo.BackColor = Color.Red;
                        toolStripStatusLabelSpacer.BackColor = toolStripStatusLabelVnaInfo.BackColor;
                        toolStripStatusLabelVnaInfo.Text = "VNA NOT CONNECTED";

                        // disable ui
                        panelMain.Enabled = false;

                        break;

                    case ComConnectionStateEnum.CONNECTED_VNA_NOT_READY:

                        // update vna info text box
                        toolStripStatusLabelVnaInfo.ForeColor = Color.White;
                        toolStripStatusLabelVnaInfo.BackColor = Color.Red;
                        toolStripStatusLabelSpacer.BackColor = toolStripStatusLabelVnaInfo.BackColor;
                        toolStripStatusLabelVnaInfo.Text = "VNA NOT READY";

                        // disable ui
                        panelMain.Enabled = false;

                        break;

                    case ComConnectionStateEnum.CONNECTED_VNA_READY:

                        // get vna info
                        Program.vna.PopulateInfo(Program.vna.app.NAME);

                        // update vna info text box
                        toolStripStatusLabelVnaInfo.ForeColor = SystemColors.ControlText;
                        toolStripStatusLabelVnaInfo.BackColor = SystemColors.Control;
                        toolStripStatusLabelSpacer.BackColor = toolStripStatusLabelVnaInfo.BackColor;
                        toolStripStatusLabelVnaInfo.Text = Program.vna.modelString + "   " + "SN:" + Program.vna.serialNumberString + "   " + Program.vna.versionString;

                        // enable ui
                        panelMain.Enabled = true;

                        break;
                }
            }

            updateUserMessage();
        }

        // ------------------------------------------------------------------------------------------------------------

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if ((comConnectionState == ComConnectionStateEnum.CONNECTED_VNA_READY) &&
                (comboBoxChannel.DroppedDown == false) &&
                (comboBoxTrace.DroppedDown == false))
            {
                updateChanComboBox();
            }
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //
        // User Message
        //
        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void updateUserMessage()
        {
            if (!(comConnectionState == ComConnectionStateEnum.CONNECTED_VNA_READY))
            {
                labelUserMessage.Visible = false;
                labelUserMessage.Text = "";
                buttonPlot.Enabled = false;
                buttonAutoConfig.Enabled = false;
            }
            else if (traceAbsoluteMeasurementConfigurationError == true)
            {
                labelUserMessage.Text = "Please Select an S-Parameter Measurement\nfor All Traces";
                labelUserMessage.Visible = true;
                buttonPlot.Enabled = false;
                buttonAutoConfig.Enabled = true;
            }
            else if (traceNonTwoPortConfigurationError == true)
            {
                labelUserMessage.Text = "Please Configure a Two-Port Measurement\n(S11, S21, S12 and S22)";
                labelUserMessage.Visible = true;
                buttonPlot.Enabled = false;
                buttonAutoConfig.Enabled = true;
            }
            else
            {
                labelUserMessage.Text = "";
                labelUserMessage.Visible = false;
                buttonPlot.Enabled = true;
                buttonAutoConfig.Enabled = true;
            }
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //
        // Channel
        //
        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void updateChanComboBox()
        {
            // save previously selected channel index
            int selectedChannelIndex = comboBoxChannel.SelectedIndex;

            // prevent combo box from flickering when update occurs
            comboBoxChannel.BeginUpdate();

            // clear channel selection combo box
            comboBoxChannel.Items.Clear();

            long splitIndex = 0;
            long activeChannel = 0;
            try
            {
                // get the split index (needed to determine number of channels)
                splitIndex = Program.vna.app.SCPI.DISPlay.SPLit;

                // determine the active channel
                activeChannel = Program.vna.app.SCPI.SERVice.CHANnel.ACTive;
            }
            catch (COMException e)
            {
                // display error message
                showMessageBoxForComException(e);
                return;
            }

            // determine number of channels from the split index
            int numOfChannels = Program.vna.DetermineNumberOfChannels(splitIndex);

            // populate the channel number combo box
            for (int ch = 1; ch < numOfChannels + 1; ch++)
            {
                comboBoxChannel.Items.Add(ch.ToString());
            }

            if ((selectedChannelIndex == -1) ||
                (selectedChannelIndex >= comboBoxChannel.Items.Count))
            {
                // init channel selection to the active channel
                comboBoxChannel.Text = activeChannel.ToString();
            }
            else
            {
                // restore previous channel selection
                comboBoxChannel.SelectedIndex = selectedChannelIndex;
            }

            // prevent combo box from flickering when update occurs
            comboBoxChannel.EndUpdate();
        }

        private void chanComboBox_SelectedIndexChanged(object sender, EventArgs args)
        {
            // save previously selected trace index
            int selectedTraceIndex = comboBoxTrace.SelectedIndex;

            // init error flags
            traceAbsoluteMeasurementConfigurationError = false;
            traceNonTwoPortConfigurationError = false;

            // determine selected channel
            string selectedItem = (string)comboBoxChannel.SelectedItem;
            int.TryParse(selectedItem, out selectedChannel);

            long numOfTraces = 1;
            bool isErrorCorrectionOn = false;
            try
            {
                // get number of traces for this channel
                numOfTraces = Program.vna.app.SCPI.CALCulate[selectedChannel].PARameter.COUNt;

                // get error correction state for this channel
                isErrorCorrectionOn = Program.vna.app.SCPI.SENSe[selectedChannel].CORRection.STATe;
            }
            catch (COMException e)
            {
                // display error message
                showMessageBoxForComException(e);
                return;
            }

            // prevent combo box from flickering when update occurs
            comboBoxTrace.BeginUpdate();

            // clear trace selection combo box
            comboBoxTrace.Items.Clear();

            // loop thru all traces on the selected channel
            List<string> traceMeasParameterList = new List<string>();
            bool fullTwoPortCalibrationDetected = false;
            for (int tr = 1; tr < numOfTraces + 1; tr++)
            {
                string traceMeasParameter = "";
                object[] correctionData;
                try
                {
                    // get this trace's measurement parameter
                    traceMeasParameter = Program.vna.app.SCPI.CALCulate[selectedChannel].PARameter[tr].DEFine;

                    // is full 2–port calibration detected on any trace?
                    if (fullTwoPortCalibrationDetected == false)
                    {
                        // no... is error correction on?
                        if (isErrorCorrectionOn == true)
                        {
                            // yes... get correction data for this trace
                            correctionData = Program.vna.app.SCPI.SENSe[selectedChannel].CORRection.TYPE[tr];

                            // check for full 2–port calibration depending on family type
                            if (Program.vna.family == VnaFamilyEnum.S2)
                            {
                                if ((string)correctionData[0] == "SOLT2")
                                {
                                    // full 2–port calibration is detected
                                    fullTwoPortCalibrationDetected = true;
                                }
                            }
                            else if (Program.vna.family == VnaFamilyEnum.S4)
                            {
                                if ((string)correctionData[0] == "SOLT4")
                                {
                                    // full 2–port calibration is detected
                                    fullTwoPortCalibrationDetected = true;
                                }
                                else if ((string)correctionData[0] == "SOLT3")
                                {
                                    // get a list of calibrated ports
                                    List<int> correctionDataList = new List<int>();
                                    correctionDataList.Add((int)correctionData[1]);
                                    correctionDataList.Add((int)correctionData[2]);
                                    correctionDataList.Add((int)correctionData[3]);

                                    // are ports 1 and 2 calibrated?
                                    if ((correctionDataList.IndexOf(1) != -1) &&
                                        (correctionDataList.IndexOf(2) != -1))
                                    {
                                        // yes... full 2–port calibration is detected on ports 1 & 2
                                        fullTwoPortCalibrationDetected = true;
                                    }
                                }
                                else if ((string)correctionData[0] == "SOLT2")
                                {
                                    // determine which ports are calibrated
                                    List<int> correctionDataList = new List<int>();
                                    correctionDataList.Add((int)correctionData[1]);
                                    correctionDataList.Add((int)correctionData[2]);

                                    // are ports 1 and 2 calibrated?
                                    if ((correctionDataList.IndexOf(1) != -1) &&
                                        (correctionDataList.IndexOf(2) != -1))
                                    {
                                        // yes... full 2–port calibration is detected on ports 1 & 2
                                        fullTwoPortCalibrationDetected = true;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (COMException)
                {
                }

                // 'absolute' measurement trace configuration?
                if (traceMeasParameter.StartsWith("S") == false)
                {
                    // yes... set error flag
                    traceAbsoluteMeasurementConfigurationError = true;
                }
                else
                {
                    // add to the measurement parameter list
                    traceMeasParameterList.Add(traceMeasParameter);
                }

                // populate trace selection combo box
                comboBoxTrace.Items.Add(tr.ToString());
            }

            // prevent combo box from flickering when update occurs
            comboBoxTrace.EndUpdate();

            // are the traces configuration for at least two-ports?
            if (!(traceMeasParameterList.Any(str => str.Contains("S11"))) ||
                !(traceMeasParameterList.Any(str => str.Contains("S21"))) ||
                !(traceMeasParameterList.Any(str => str.Contains("S12"))) ||
                !(traceMeasParameterList.Any(str => str.Contains("S22"))))
            {
                // no... is full 2–port calibration detected?
                if (fullTwoPortCalibrationDetected == false)
                {
                    // no... set error flag
                    traceNonTwoPortConfigurationError = true;
                }
            }

            if ((selectedTraceIndex == -1) ||
                (selectedTraceIndex >= comboBoxTrace.Items.Count) ||
                (didAutoConfig == true))
            {
                // init trace selection
                comboBoxTrace.SelectedIndex = comboBoxTrace.Items.Count - 1;

                // reset auto config flag
                didAutoConfig = false;
            }
            else
            {
                // restore previous trace selection
                comboBoxTrace.SelectedIndex = selectedTraceIndex;
            }

            // update user message label
            updateUserMessage();
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //
        // Auto Configuration
        //
        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void autoConfigButton_Click(object sender, EventArgs args)
        {
            // display warning message
            DialogResult dialogResult = MessageBox.Show("Do you want to automatically configure traces for plotting amplifier stability?\n\nThis will cause the loss of your current trace configuration on the selected channel.",
                Program.programName,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dialogResult == DialogResult.Yes)
            {
                // yes...
                performAutoConfig();
            }
        }

        private void performAutoConfig()
        {
            // disabled buttons (they will be enabled upon next update)
            buttonAutoConfig.Enabled = false;
            buttonPlot.Enabled = false;

            try
            {
                object err;

                // configure five traces
                Program.vna.app.SCPI.CALCulate[selectedChannel].PARameter.COUNt = 5;

                // 1st trace (S11)
                Program.vna.app.SCPI.CALCulate[selectedChannel].PARameter[1].DEFine = "S11";

                // 2nd trace (S21)
                Program.vna.app.SCPI.CALCulate[selectedChannel].PARameter[2].DEFine = "S21";

                // 3rd trace (S12)
                Program.vna.app.SCPI.CALCulate[selectedChannel].PARameter[3].DEFine = "S12";

                // 4th trace (S22)
                Program.vna.app.SCPI.CALCulate[selectedChannel].PARameter[4].DEFine = "S22";

                // set configuration common to the first four traces
                for (int i = 1; i < 4; i++)
                {
                    Program.vna.app.SCPI.DISPlay.WINDow[selectedChannel].TRACe[i].STATe = true;
                    Program.vna.app.SCPI.DISPlay.WINDow[selectedChannel].TRACe[i].MEMory.STATe = false;
                    err = Program.vna.app.SCPI.CALCulate[selectedChannel].PARameter[i].SELect;
                    Program.vna.app.SCPI.CALCulate[selectedChannel].SELected.FORMat = "MLOGarithmic";
                }

                // 5th trace (calculated amplifier stability)
                Program.vna.app.SCPI.CALCulate[selectedChannel].PARameter[5].DEFine = "S11";
                Program.vna.app.SCPI.DISPlay.WINDow[selectedChannel].TRACe[5].STATe = false;
                err = Program.vna.app.SCPI.CALCulate[selectedChannel].PARameter[5].SELect;
                Program.vna.app.SCPI.CALCulate[selectedChannel].SELected.FORMat = "MLINear";

                // allocate traces
                Program.vna.app.SCPI.DISPlay.WINDow[selectedChannel].SPLit = 9;

                // set auto config flag
                didAutoConfig = true;
            }
            catch (COMException e)
            {
                // display error message
                showMessageBoxForComException(e);
                return;
            }
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //
        // Plot Amplifier Stability
        //
        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void plotButton_Click(object sender, EventArgs args)
        {
            // stop the update timer
            updateTimer.Stop();

            // --------------------------------------------------------------------------------------------------------

            // previous trigger continuous state
            string previousTriggerContinuousState = "";

            // previous trigger source
            string previousTriggerSource = "";

            // previous measurement parameter for trace 1
            string previousTrace1MeasParameter = "";

            // measurement parameter of the selected trace
            string selectedTraceMeasParameter = "";

            // frequency data
            double[] F;

            // s-parameter data
            double[] S11;
            double[] S12;
            double[] S13;
            double[] S14;
            double[] S21;
            double[] S22;
            double[] S23;
            double[] S24;
            double[] S31;
            double[] S32;
            double[] S33;
            double[] S34;
            double[] S41;
            double[] S42;
            double[] S43;
            double[] S44;

            // --------------------------------------------------------------------------------------------------------

            // get the selected trace number
            int selectedTrace = comboBoxTrace.SelectedIndex + 1;

            // --------------------------------------------------------------------------------------------------------

            try
            {
                // activate the selected channel
                object err = Program.vna.app.SCPI.DISPlay.WINDow[selectedChannel].ACTivate;

                // get the selected trace measurement parameter
                selectedTraceMeasParameter = Program.vna.app.SCPI.CALCulate[selectedChannel].PARameter[selectedTrace].DEFine;

                // save the trigger continuous state
                bool isContinuous = Program.vna.app.SCPI.INITiate[selectedChannel].CONTinuous;
                previousTriggerContinuousState = isContinuous.ToString();

                // set trigger continuous state to false
                Program.vna.app.SCPI.INITiate[selectedChannel].CONTinuous = false;

                // save the trigger source
                previousTriggerSource = Program.vna.app.SCPI.TRIGger.SEQuence.SOURce;

                // set trigger source to BUS
                Program.vna.app.SCPI.TRIGger.SEQuence.SOURce = "BUS";

                // generate a single trigger and wait for completion
                err = Program.vna.app.SCPI.INITiate[selectedChannel].IMMediate;
                err = Program.vna.app.SCPI.TRIGger.SEQuence.SINGle;

                // retrieve frequency data
                F = Program.vna.app.SCPI.SENSe.Frequency.Data;

                // save the measurement parameter for trace 1
                previousTrace1MeasParameter = Program.vna.app.SCPI.CALCulate[selectedChannel].PARameter[1].DEFine;

                // retrieve s11-parameter data
                Program.vna.app.SCPI.Calculate[selectedChannel].Parameter[1].DEFine = "S11";
                S11 = Program.vna.app.SCPI.Calculate[selectedChannel].TRACe[1].Data.SDATa;

                // retrieve s21-parameter data
                Program.vna.app.SCPI.Calculate[selectedChannel].Parameter[1].DEFine = "S21";
                S21 = Program.vna.app.SCPI.Calculate[selectedChannel].TRACe[1].Data.SDATa;

                // retrieve s12-parameter data
                Program.vna.app.SCPI.Calculate[selectedChannel].Parameter[1].DEFine = "S12";
                S12 = Program.vna.app.SCPI.Calculate[selectedChannel].TRACe[1].Data.SDATa;

                // retrieve s22-parameter data
                Program.vna.app.SCPI.Calculate[selectedChannel].Parameter[1].DEFine = "S22";
                S22 = Program.vna.app.SCPI.Calculate[selectedChannel].TRACe[1].Data.SDATa;

                // restore the previous measurement parameter for trace 1
                Program.vna.app.SCPI.CALCulate[selectedChannel].PARameter[1].DEFine = previousTrace1MeasParameter;

                // restore the previous trigger source
                Program.vna.app.SCPI.TRIGger.SEQuence.SOURce = previousTriggerSource;

                // restore the trigger continuous state
                Program.vna.app.SCPI.INITiate[selectedChannel].CONTinuous = Convert.ToBoolean(previousTriggerContinuousState);
            }
            catch (COMException e)
            {
                // attempt to restore the settings we modified
                if (string.IsNullOrEmpty(previousTrace1MeasParameter) == false)
                {
                    // restore the previous measurement parameter for trace 1
                    Program.vna.app.SCPI.CALCulate[selectedChannel].PARameter[1].DEFine = previousTrace1MeasParameter;
                }
                if (string.IsNullOrEmpty(previousTriggerSource) == false)
                {
                    // restore the previous trigger source
                    Program.vna.app.SCPI.TRIGger.SEQuence.SOURce = previousTriggerSource;
                }
                if (string.IsNullOrEmpty(previousTriggerContinuousState) == false)
                {
                    // restore the trigger continuous state
                    Program.vna.app.SCPI.INITiate[selectedChannel].CONTinuous = Convert.ToBoolean(previousTriggerContinuousState);
                }

                // display error message
                showMessageBoxForComException(e);
                return;
            }

            // --------------------------------------------------------------------------------------------------------

            // calculated amplifier stability data
            double[] calculatedAmplifierStability = new double[F.Length];

            // calculate k parameter
            if (radioButtonK.Checked == true)
            {
                for (int i = 0; i < F.Length; i++)
                {
                    Complex complexS11 = new Complex(S11[i * 2], S11[i * 2 + 1]);
                    Complex complexS21 = new Complex(S21[i * 2], S21[i * 2 + 1]);
                    Complex complexS12 = new Complex(S12[i * 2], S12[i * 2 + 1]);
                    Complex complexS22 = new Complex(S22[i * 2], S22[i * 2 + 1]);

                    double magnitudeDelta = Complex.Abs(complexS11 * complexS22 - complexS12 * complexS21);
                    double magnitudeS11 = Complex.Abs(complexS11);
                    double magnitudeS22 = Complex.Abs(complexS22);

                    double numerator = 1 - (magnitudeS11 * magnitudeS11) - (magnitudeS22 * magnitudeS22) + (magnitudeDelta * magnitudeDelta);
                    double denominator = 2 * Complex.Abs(complexS12 * complexS21);
                    calculatedAmplifierStability[i] = numerator / denominator;
                }
            }

            // calculate mu1 parameter
            else if (radioButtonMu1.Checked == true)
            {
                for (int i = 0; i < F.Length; i++)
                {
                    Complex complexS11 = new Complex(S11[i * 2], S11[i * 2 + 1]);
                    Complex complexS21 = new Complex(S21[i * 2], S21[i * 2 + 1]);
                    Complex complexS12 = new Complex(S12[i * 2], S12[i * 2 + 1]);
                    Complex complexS22 = new Complex(S22[i * 2], S22[i * 2 + 1]);

                    Complex conjugateS11 = Complex.Conjugate(complexS11);

                    Complex complexDelta = complexS11 * complexS22 - complexS12 * complexS21;
                    double magnitudeS11 = Complex.Abs(complexS11);
                    double magnitudeS22 = Complex.Abs(complexS22);

                    double numerator = 1 - (magnitudeS11 * magnitudeS11);
                    double denominator = Complex.Abs(conjugateS11 * complexDelta - complexS22) + Complex.Abs(complexS12 * complexS21);
                    calculatedAmplifierStability[i] = numerator / denominator;
                }
            }

            // calculate mu2 parameter
            else if (radioButtonMu2.Checked == true)
            {
                for (int i = 0; i < F.Length; i++)
                {
                    Complex complexS11 = new Complex(S11[i * 2], S11[i * 2 + 1]);
                    Complex complexS21 = new Complex(S21[i * 2], S21[i * 2 + 1]);
                    Complex complexS12 = new Complex(S12[i * 2], S12[i * 2 + 1]);
                    Complex complexS22 = new Complex(S22[i * 2], S22[i * 2 + 1]);

                    Complex conjugateS22 = Complex.Conjugate(complexS22);

                    Complex complexDelta = complexS11 * complexS22 - complexS12 * complexS21;
                    double magnitudeS11 = Complex.Abs(complexS11);
                    double magnitudeS22 = Complex.Abs(complexS22);

                    double numerator = 1 - (magnitudeS22 * magnitudeS22);
                    double denominator = Complex.Abs(conjugateS22 * complexDelta - complexS11) + Complex.Abs(complexS12 * complexS21);
                    calculatedAmplifierStability[i] = numerator / denominator;
                }
            }

            // --------------------------------------------------------------------------------------------------------

            // initialize s-parameter data arrays
            S11 = new double[F.Length];
            S12 = new double[F.Length];
            S13 = new double[F.Length];
            S14 = new double[F.Length];
            S21 = new double[F.Length];
            S22 = new double[F.Length];
            S23 = new double[F.Length];
            S24 = new double[F.Length];
            S31 = new double[F.Length];
            S32 = new double[F.Length];
            S33 = new double[F.Length];
            S34 = new double[F.Length];
            S41 = new double[F.Length];
            S42 = new double[F.Length];
            S43 = new double[F.Length];
            S44 = new double[F.Length];

            // clone calculated amplifier stability results to appropriate array
            switch (selectedTraceMeasParameter)
            {
                default:
                case "S11":
                    S11 = (double[])calculatedAmplifierStability.Clone();
                    break;

                case "S12":
                    S12 = (double[])calculatedAmplifierStability.Clone();
                    break;

                case "S13":
                    S13 = (double[])calculatedAmplifierStability.Clone();
                    break;

                case "S14":
                    S14 = (double[])calculatedAmplifierStability.Clone();
                    break;

                case "S21":
                    S21 = (double[])calculatedAmplifierStability.Clone();
                    break;

                case "S22":
                    S22 = (double[])calculatedAmplifierStability.Clone();
                    break;

                case "S23":
                    S23 = (double[])calculatedAmplifierStability.Clone();
                    break;

                case "S24":
                    S24 = (double[])calculatedAmplifierStability.Clone();
                    break;

                case "S31":
                    S31 = (double[])calculatedAmplifierStability.Clone();
                    break;

                case "S32":
                    S32 = (double[])calculatedAmplifierStability.Clone();
                    break;

                case "S33":
                    S33 = (double[])calculatedAmplifierStability.Clone();
                    break;

                case "S34":
                    S34 = (double[])calculatedAmplifierStability.Clone();
                    break;

                case "S41":
                    S41 = (double[])calculatedAmplifierStability.Clone();
                    break;

                case "S42":
                    S42 = (double[])calculatedAmplifierStability.Clone();
                    break;

                case "S43":
                    S43 = (double[])calculatedAmplifierStability.Clone();
                    break;

                case "S44":
                    S44 = (double[])calculatedAmplifierStability.Clone();
                    break;
            }

            // --------------------------------------------------------------------------------------------------------

            // make sure touchstone file directory exists
            string directoryPath = @"FixtureSim";
            // directoryPath = @"C:\VNA\S4VNA\FixtureSim"; // for testing S4
            // directoryPath = @"C:\VNA\S2VNA\FixtureSim"; // for testing S2

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // write a touchstone file to the hard drive
            string fileName = "AmplifierStability";
            string fileExtension = ".s2p";
            if (Program.vna.family == VnaFamilyEnum.S4)
            {
                fileExtension = ".s4p";
            }
            string filePath = directoryPath + @"\" + fileName + fileExtension;
            using (TextWriter writer = File.CreateText(filePath))
            {
                writer.WriteLine("# Hz S RI R 50");
                for (int i = 0; i < F.Length; i++)
                {
                    if (Program.vna.family == VnaFamilyEnum.S2)
                    {
                        writer.WriteLine(string.Format("{0:#.00000000000E+00}" + " " +
                                                       "{1:+#.00000000000E+00;-#.00000000000E+00} {2:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       "{3:+#.00000000000E+00;-#.00000000000E+00} {4:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       "{5:+#.00000000000E+00;-#.00000000000E+00} {6:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       "{7:+#.00000000000E+00;-#.00000000000E+00} {8:+#.00000000000E+00;-#.00000000000E+00}",
                                                       F[i],
                                                       S11[i], 0,
                                                       S21[i], 0,
                                                       S12[i], 0,
                                                       S22[i], 0));
                    }
                    else
                    {
                        writer.WriteLine(string.Format("{0:#.00000000000E+00}" + " " +
                                                       "{1:+#.00000000000E+00;-#.00000000000E+00} {2:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       "{3:+#.00000000000E+00;-#.00000000000E+00} {4:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       "{5:+#.00000000000E+00;-#.00000000000E+00} {6:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       "{7:+#.00000000000E+00;-#.00000000000E+00} {8:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       Environment.NewLine +
                                                       "                  {9:+#.00000000000E+00;-#.00000000000E+00} {10:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       "{11:+#.00000000000E+00;-#.00000000000E+00} {12:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       "{13:+#.00000000000E+00;-#.00000000000E+00} {14:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       "{15:+#.00000000000E+00;-#.00000000000E+00} {16:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       Environment.NewLine +
                                                       "                  {17:+#.00000000000E+00;-#.00000000000E+00} {18:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       "{19:+#.00000000000E+00;-#.00000000000E+00} {20:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       "{21:+#.00000000000E+00;-#.00000000000E+00} {22:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       "{23:+#.00000000000E+00;-#.00000000000E+00} {24:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       Environment.NewLine +
                                                       "                  {25:+#.00000000000E+00;-#.00000000000E+00} {26:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       "{27:+#.00000000000E+00;-#.00000000000E+00} {28:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       "{29:+#.00000000000E+00;-#.00000000000E+00} {30:+#.00000000000E+00;-#.00000000000E+00}" + " " +
                                                       "{31:+#.00000000000E+00;-#.00000000000E+00} {32:+#.00000000000E+00;-#.00000000000E+00}",
                                                       F[i],
                                                       S11[i], 0,
                                                       S12[i], 0,
                                                       S13[i], 0,
                                                       S14[i], 0,
                                                       S21[i], 0,
                                                       S22[i], 0,
                                                       S23[i], 0,
                                                       S24[i], 0,
                                                       S31[i], 0,
                                                       S32[i], 0,
                                                       S33[i], 0,
                                                       S34[i], 0,
                                                       S41[i], 0,
                                                       S42[i], 0,
                                                       S43[i], 0,
                                                       S44[i], 0));
                    }
                }
            }

            // --------------------------------------------------------------------------------------------------------

            // calculate range
            double minValue = calculatedAmplifierStability.Min();
            double maxValue = calculatedAmplifierStability.Max();
            double range = maxValue - minValue;

            // --------------------------------------------------------------------------------------------------------

            try
            {
                object err;

                // set selected trace on the vna
                err = Program.vna.app.SCPI.CALCulate[selectedChannel].PARameter[selectedTrace].SELect;

                // set the selected trace data format to linear
                Program.vna.app.SCPI.CALCulate[selectedChannel].SELected.FORMat = "MLINear";

                // turn on live data on the selected trace
                Program.vna.app.SCPI.DISPlay.WINDow[selectedChannel].TRACe[selectedTrace].STATe = true;

                // load the touchstone file on the vna
                Program.vna.app.SCPI.MMEMory.LOAD.SNP.TRACe[selectedTrace].MEMory = filePath;

                // turn off live data on the selected trace
                Program.vna.app.SCPI.DISPlay.WINDow[selectedChannel].TRACe[selectedTrace].STATe = false;

                // get number of divisions
                long divisions = Program.vna.app.SCPI.DISPlay.WINDow[selectedChannel].Y.SCALe.DIVisions;

                // auto scale
                double scale = range / (divisions - 1);
                scale = Math.Ceiling(scale);
                Program.vna.app.SCPI.DISPlay.WINDow[selectedChannel].TRACe[selectedTrace].Y.SCALe.PDIVision = scale;

                // auto reference level
                double refLevel = maxValue - (scale * (divisions / 2));
                refLevel = Math.Round(refLevel, 1);
                Program.vna.app.SCPI.DISPlay.WINDow[selectedChannel].TRACe[selectedTrace].Y.SCALe.RLEVel = refLevel;
            }
            catch (COMException e)
            {
                // display error message
                showMessageBoxForComException(e);
                return;
            }
            finally
            {
                // re-start the update timer
                updateTimer.Start();
            }
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void showMessageBoxForComException(COMException e)
        {
            MessageBox.Show(Program.vna.GetUserMessageForComException(e),
                Program.programName,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    }
}