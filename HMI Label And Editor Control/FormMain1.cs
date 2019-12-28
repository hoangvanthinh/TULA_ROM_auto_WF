using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using EasyModbus;

using System.Drawing.Imaging;
//using AForge;
//using AForge.Imaging;
//using AForge.Video;
//using AForge.Video.DirectShow;
//using AForge.Imaging.Filters;
//using AForge.Math.Geometry;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;


namespace How_to_create_HMI_Control_Real_Time
{
    public partial class FormMain : Form
    {
        ModbusClient MB;
        const int ADD_X = 1;
        const int ADD_Y = 2;
        const int ADD_Z = 3;
        const int ADD_R1 = 4;
        const int ADD_R2 = 5;

        const int ADD_HOME = 6;
        const int ADD_AM = 7;


        const int ADD_REQ_X = 11;
        const int ADD_REQ_Y = 12;
        const int ADD_REQ_Z = 13;
        const int ADD_REQ_R1 = 14;
        const int ADD_REQ_R2 = 15;

        const int ADD_PUMP = 19;
        const int ADD_VACUM1 = 20;
        const int ADD_VACUM2 = 21;
        const int ADD_LED = 22;
        //============================ CAMERA ===========================

        Image<Bgr, byte> imgInput;
        Capture capture;
        float toado_x, toado_y, gocquay;
        float vX, vY, vR;
        int check_cam_status = 0;
        int Cam_check_ok = 0, req_CAM = 0;
        int solan_chup = 0;
        int rong = 0, dai = 0;
        float Wchip = 0;
        int CAM_CHOICE = 1;
        const int CAM_BOT = 1, CAM_TOP = 2;
        //----------------------------------- actua ------------------------------
        ushort _AM = 0;
        const int _Auto = 1, _Manual = 0;
        const int _X = 1, _Y = 2, _Z = 3, _R1 = 4, _R2 = 5;

        const int _HOME = 6, _Add_AutoManual = 7, _Add_UPS = 8, _Add_COOD = 9, _Add_SPEED = 10;
        const int _Add_Xilanh = 23, _Add_LED = 22;
        const int _Add_CAM = 24;

        ushort _SPEED = 0;
        const int SPEED_LOW = 0, SPEED_HIGH = 1;
        ushort _ups = 0, _coodinate = 0;
        ushort Start = 0;
        const int PUMP_ON = 0, PUMP_OFF = 1;
        const int VACUM_1_ON = 1, VACUM_1_OFF = 0;
        const int VACUM_2_ON = 1, VACUM_2_OFF = 0;
        const int LED_ON = 1, LED_OFF = 0;
        const int XL1_UP = 1, XL1_DOWN = 0;
        ushort _Status_PUMP = PUMP_OFF, _Status_VACUM_1 = 0, _Status_VACUM_2 = 0, _Status_LED = 0, _Status_XL1 = 0;

        //--------------------- di chuyen tu dong--------------------------
        ushort _PAUSE = 0;
        ushort _coodinateX = 0, _coodinateY = 0, _coodinateZ = 0, _coodinateR1 = 0, _coodinateR2 = 0;
        int _runX = 0, _runY = 0, _runZ = 0, _runR1 = 0, _runR2 = 0;
        int _step = 0, _step1 = 0;  // CAC BUOC DI CHUYEN TUAN TU
        int reqX = 0, reqY = 0, reqZ = 0, reqR1 = 0, reqR2 = 0;

        int[] X_TRAY_IC = new int[100]; // X
        int[] Y_TRAY_IC = new int[100]; // Y
        int Z_TRAY_IC = 0;
        int X_START_IC = 0, X_END_IC = 0, Y_START_IC = 0, Y_END_IC = 0;

        int[] X_TRAY_ROM = new int[100]; // X
        int[] Y_TRAY_ROM = new int[100]; // Y
        int Z_TRAY_ROM = 0;
        int X_START_ROM = 0, X_END_ROM = 0, Y_START_ROM = 0, Y_END_ROM = 0;

        int[] X_TRAY_PASS = new int[100]; // X
        int[] Y_TRAY_PASS = new int[100]; // Y
        int Z_TRAY_PASS = 0;
        int X_START_PASS = 0, X_END_PASS = 0, Y_START_PASS = 0, Y_END_PASS = 0;

        int[] X_TRAY_NG = new int[100]; // X
        int[] Y_TRAY_NG = new int[100]; // Y
        int Z_TRAY_NG = 0;
        int X_START_NG = 0, X_END_NG = 0, Y_START_NG = 0, Y_END_NG = 0;

        int X_CAM = 0;
        int Y_CAM = 0;

        ushort AUTO_RUN = 0, _READY_RUN = 0, _Req_ORG = 0;
        ushort i_IC = 0, i_in_ROM = 0,i_out_ROM = 0, i_NG = 0, i_PASS = 0;
        //---------------------------------------------------------------------
        int IC_Stack_choice = 0;
        const int IC_START_CHOICE = 1, IC_END_CHOICE = 2;
        int ROM_Stack_choice = 0;
        const int ROM_START_CHOICE = 1, ROM_END_CHOICE = 2;
        int NG_Stack_choice = 0;
        const int NG_START_CHOICE = 1, NG_END_CHOICE = 2;
        int PASS_Stack_choice = 0;
        const int PASS_START_CHOICE = 1, PASS_END_CHOICE = 2;
        int CAM_Stack_choice = 0;
        const int CAM_START_CHOICE = 1, CAM_END_CHOICE = 2;

        //ushort IC_Stack_sohang = 0, IC_Stack_socot = 0;
        //ushort ROM_Stack_sohang = 0, ROM_Stack_socot = 0;
        // ----------------- SAVE DATA -------------------------------
        byte[] data = new byte[20];
        //=============================== ROM ============================
        int Nap_rom = 0;
        int Full_Stack_Rom =0;
        int ROM_WRITED = 0;
        //private Register _Register = null;
        Bitmap TULA = new Bitmap(650, 550);
        public FormMain()
        {
            InitializeComponent();
            Installing.Hide();

            IC_Start_choice.Hide();
            IC_End_choice.Hide();
            ROM_End_choice.Hide();
            ROM_Start_choice.Hide();
            NG_End_choice.Hide();
            NG_Start_choice.Hide();
            PASS_End_choice.Hide();
            PASS_Start_choice.Hide();

            CAM_Start_choice.Hide();
            //Line line = Line.FormMain(new Point(0, 0), new Point(3, 4));
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {

                MB = new ModbusClient("COM4");
                MB.UnitIdentifier = 1;
                MB.Baudrate = 115200;
                MB.Parity = System.IO.Ports.Parity.None;
                MB.StopBits = System.IO.Ports.StopBits.One;
                MB.ConnectionTimeout = 200;
                MB.Connect();
                MB.WriteSingleRegister(ADD_REQ_R1, 800);

                timer1.Enabled = true;
            
                Start_CAM();


            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Speed_choice.SelectedIndex = Speed_choice.FindString("5");
        }

        private void Start_MB()
        {
            //throw new NotImplementedException();
            Thread newThread = new Thread((obj) =>
            {
                while (true)
                {
                    //_coodinateX = (ushort)MB.ReadHoldingRegisters(ADD_X, 1)[0];
                    //_coodinateY = (ushort)MB.ReadHoldingRegisters(ADD_Y, 1)[0];
                    //_coodinateZ = (ushort)MB.ReadHoldingRegisters(ADD_Z, 1)[0];
                    //_coodinateR1 = (ushort)MB.ReadHoldingRegisters(ADD_R1, 1)[0];
                    //_coodinateR2 = (ushort)MB.ReadHoldingRegisters(ADD_R2, 1)[0];

                    //Toado_X.Text = _coodinateX.ToString();
                    //Toado_Y.Text = _coodinateY.ToString();
                    //Toado_Z.Text = _coodinateZ.ToString();
                    //Toado_R1.Text = _coodinateR1.ToString();
                    //Toado_R2.Text = _coodinateR2.ToString();

                    //Status_Home.Text = MB.ReadHoldingRegisters(ADD_HOME, 1)[0].ToString();
                    //Auto_manual.Text = MB.ReadHoldingRegisters(ADD_AM, 1)[0].ToString();

                    //UpDownStop.Text = MB.ReadHoldingRegisters(8, 1)[0].ToString();
                    //Dichuyen_Truc.Text = MB.ReadHoldingRegisters(9, 1)[0].ToString();
                    //Tocdo_dichuyen.Text = MB.ReadHoldingRegisters(10, 1)[0].ToString();


                    //reqX = MB.ReadHoldingRegisters(ADD_REQ_X, 1)[0];
                    //reqY = MB.ReadHoldingRegisters(ADD_REQ_Y, 1)[0];
                    //reqZ = MB.ReadHoldingRegisters(ADD_REQ_Z, 1)[0];
                    //reqR1 = MB.ReadHoldingRegisters(ADD_REQ_R1, 1)[0];
                    //reqR2 = MB.ReadHoldingRegisters(ADD_REQ_R2, 1)[0];

                    //Req_X.Text = reqX.ToString();
                    //Req_Y.Text = reqY.ToString();
                    //Req_Z.Text = reqZ.ToString();
                    //Req_R1.Text = reqR1.ToString();
                    //Req_R2.Text = reqR2.ToString();
                    Thread.Sleep(50); // Delay 100ms
                }
            });
            newThread.IsBackground = true;
            newThread.Start();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Show("KeyDown");
        }

        private void Req_GOHOME()
        {
            //throw new NotImplementedException();
            try
            {
                MB.WriteSingleRegister(_HOME, 1); // Max= 65535
            }
            catch
            {

            }
        }

        private void button5_MouseDown(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 1;
                _coodinate = _X;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }


            }
        }

        private void button5_MouseUp(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 0;
                _coodinate = _X;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }

        private void button4_MouseDown(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 2;
                _coodinate = _X;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }

        private void button4_MouseUp(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 0;
                _coodinate = _X;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }


        private void Y_UP_MouseDown(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 1;
                _coodinate = _Y;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }

        private void Y_UP_MouseUp(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 0;
                _coodinate = _Y;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }

        private void button3_MouseDown(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 2;
                _coodinate = _Y;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }

        private void button3_MouseUp(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 0;
                _coodinate = _Y;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }

        private void button7_MouseDown(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 1;
                _coodinate = _Z;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }

        private void button7_MouseUp(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 0;
                _coodinate = _Z;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }

        private void button6_MouseDown(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 2;
                _coodinate = _Z;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }

        private void button6_MouseUp(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 0;
                _coodinate = _Z;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System_config systemCF = new System_config();
            systemCF.Show();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            
            if (capture != null)
            {
                //c_x.Text = toado_x.ToString();
                //c_y.Text = toado_y.ToString();
                //_angle.Text = gocquay.ToString();
                Xm.Text = Wchip.ToString();
                //Ym.Text = dai.ToString();

                dX.Text = "dX:" + vX.ToString();
                dY.Text = "dY:" + vY.ToString();
                dR.Text = "dR:" + gocquay.ToString();
                //Checking.Text = Cam_check_ok.ToString();
            }


            if (!MB.Connected)
            {
                if (MB.Available(50))
                {
                    //timer1.Enabled = false;
                    //MB.Disconnect();
                    //MB.Connect();
                    timer1.Enabled = true;
                }
            }
            else
            {
                try
                {
                    timer1.Enabled = false;

                    _coodinateX = (ushort)MB.ReadHoldingRegisters(ADD_X, 1)[0];
                    _coodinateY = (ushort)MB.ReadHoldingRegisters(ADD_Y, 1)[0];
                    _coodinateZ = (ushort)MB.ReadHoldingRegisters(ADD_Z, 1)[0];
                    _coodinateR1 = (ushort)MB.ReadHoldingRegisters(ADD_R1, 1)[0];
                    _coodinateR2 = (ushort)MB.ReadHoldingRegisters(ADD_R2, 1)[0];

                    Toado_X.Text = _coodinateX.ToString();
                    Toado_Y.Text = _coodinateY.ToString();
                    Toado_Z.Text = _coodinateZ.ToString();
                    Toado_R1.Text = _coodinateR1.ToString();
                    Toado_R2.Text = _coodinateR2.ToString();

                    Status_Home.Text = MB.ReadHoldingRegisters(ADD_HOME, 1)[0].ToString();
                    Auto_manual.Text = MB.ReadHoldingRegisters(ADD_AM, 1)[0].ToString();

                    UpDownStop.Text = MB.ReadHoldingRegisters(8, 1)[0].ToString();
                    Dichuyen_Truc.Text = MB.ReadHoldingRegisters(9, 1)[0].ToString();
                    Tocdo_dichuyen.Text = MB.ReadHoldingRegisters(10, 1)[0].ToString();


                    reqX = MB.ReadHoldingRegisters(ADD_REQ_X, 1)[0];
                    reqY = MB.ReadHoldingRegisters(ADD_REQ_Y, 1)[0];
                    reqZ = MB.ReadHoldingRegisters(ADD_REQ_Z, 1)[0];
                    reqR1 = MB.ReadHoldingRegisters(ADD_REQ_R1, 1)[0];
                    reqR2 = MB.ReadHoldingRegisters(ADD_REQ_R2, 1)[0];

                    Req_X.Text = reqX.ToString();
                    Req_Y.Text = reqY.ToString();
                    Req_Z.Text = reqZ.ToString();
                    Req_R1.Text = reqR1.ToString();
                    //Req_R2.Text = reqR2.ToString();
                    

                    //======= KIEM TRA ROBOT DA ORG CHUA ?? ===================================//
                    if (AUTO_RUN == 1 && _READY_RUN == 0 && _Req_ORG == 1)
                    {
                        _Req_ORG = 0;
                        //MB.WriteSingleRegister(_Add_SPEED, SPEED_HIGH);
                        try
                        {
                            //Req_GOHOME();
                        }
                        catch
                        {

                            MB.Connect();
                        }

                    }
                    if (AUTO_RUN == 1 && _coodinateX == 0 && _coodinateY == 0 && _coodinateZ == 0)
                    {
                        //=========>> BAT DAU CHAY AUTO  <<===================================//
                        _READY_RUN = 1;
                        MB.WriteSingleRegister(ADD_AM, _Auto);
                        MB.WriteSingleRegister(_Add_Xilanh, XL1_DOWN);

                        Automanual.Text = "Auto";
                        i_IC = 1;
                        i_in_ROM = 1;
                        i_out_ROM = 1;
                        i_PASS = 1;
                        i_NG = 1;
                        _step = 1;
                        _step1 = 1;
                        ROM_WRITED = 0;
                        Enable_PUMP();

                        Installing.Hide();

                        WorkTable.Enabled = true;
                    }

                    if (AUTO_RUN == 1 && _READY_RUN == 1 && _PAUSE == 0)
                    {
                        //===============================KIEM TRA TRANG THAI CAC TRUC 
                        if (reqX == _coodinateX && reqY == _coodinateY)
                        {
                            _runX = _runY = 0;
                        }
                        if (reqZ == _coodinateZ)
                            _runZ = 0;
                        if (reqR1 == _coodinateR1)
                        {
                            _runR1 = 0;
                        }
                        if (reqR2 == _coodinateR2)
                        {
                            _runR2 = 0;
                        }
                        //========================= IC TO ROM   ==========>>>>
                        if (Full_Stack_Rom == 0)
                        {
                            CAM_CHOICE = CAM_BOT;
                          MB.WriteSingleRegister(_Add_CAM, CAM_CHOICE);
                            IC_to_ROM();

                        }
                        //========================= NAP ================= >>>>
                        if(Nap_rom == 1 && Full_Stack_Rom == 1)
                        {
                            //Full_Stack_Rom = 0;
                            ROM_WRITED = 1;
                            MB.WriteSingleRegister(_Add_Xilanh, XL1_UP);

                        }
                        if (Nap_rom == 0 && Full_Stack_Rom == 1 && ROM_WRITED == 1)
                        {
                            //Full_Stack_Rom = 0;
                            ROM_WRITED = 2;
                            MB.WriteSingleRegister(_Add_Xilanh, XL1_DOWN);

                        }
                        //========================= ROM TO OK_NG =========>>>>>
                        if (ROM_WRITED == 2 && Full_Stack_Rom == 1)
                        {
                            ROM_to_OK_NG(); 
                        }
                    }


                    timer1.Enabled = true;
                }
                catch
                {
                    //MB.Disconnect();
                    //MB.Connect();
                    timer1.Enabled = true;
                }
            }


        }
        void ROM_to_OK_NG()
        {
            if (_step1 == 1)  //---------------------- LAY TRAY IC
            {
                MoveXY(X_TRAY_ROM[i_out_ROM], Y_TRAY_ROM[i_out_ROM]);
                _runX = _runY = 1;
                _step1++;
            }
            if (_step1 == 2 && _runX == 0 && _runY == 0) // XUONG NOZZLE
            {
                //MoveZ(Z_TRAY_IC);
                MoveZ(200);

                _runZ = 1;
                _step1++;
                Enable_PUMP();
            }
            if (_step1 == 3)// HUT
            {
                Enable_VACUM_1();
                // DINH THOI: HUT 10mS rồi chuyen qua step++
                _step1++;
            }
            if (_step1 == 4 && _runZ == 0)// LEN NOZZLE
            {
                MoveZ(5);
                _runZ = 1;
                _step1++;
            }
            if (_step1 == 5 && _runZ == 0) //------------------ DI CHUYEN DEN MAY NAP ROM
            {
                MoveXY(X_TRAY_PASS[i_PASS], Y_TRAY_PASS[i_PASS]);
                _runX = _runY = 1;
                _step1++;
            }
            if (_step1 == 6 && _runX == 0 && _runY == 0) // XUONG NOZZLE
            {
                MoveZ(200);
                _runZ = 1;
                // Disable_PUMP();
                Disable_VACUM_1();
                _step1++;
            }
            if (_step1 == 7 && _runZ == 0) // LEN NOZZLE
            {
                MoveZ(5);
                _runZ = 1;

                //_step++;

                i_out_ROM++;
                if (i_out_ROM > (int)ROM_Stack_cot.Value * (int)ROM_Stack_hang.Value)
                {
                    i_out_ROM = 1;
                    Full_Stack_Rom = 0;
                    ROM_WRITED = 0;
                }
                i_PASS++;
                if (i_PASS > (int)PASS_Stack_cot.Value * (int)PASS_Stack_hang.Value)
                {
                    //if (i_ROM >= 5)
                    i_PASS = 1;
                    
                }
                _step1 = 1;

            }
        }
        void IC_to_ROM()
        {
            if (_step == 1)  //---------------------- LAY TRAY IC
            {
                MoveXY(X_TRAY_IC[i_IC], Y_TRAY_IC[i_IC]);
                _runX = _runY = 1;
                _step++;
            }
            if (_step == 2 && _runX == 0 && _runY == 0) // XUONG NOZZLE
            {
                //MoveZ(Z_TRAY_IC);
                MoveZ(200);

                _runZ = 1;
                _step++;
                Enable_PUMP();
            }
            if (_step == 3)// HUT
            {
                Enable_VACUM_1();
                // DINH THOI: HUT 10mS rồi chuyen qua step++
                _step++;
            }
            if (_step == 4 && _runZ == 0)// LEN NOZZLE
            {
                MoveZ(5);
                _runZ = 1;
                _step++;
            }

            //====================== DI CHUYEN TOI CAM =========================

            if(_step == 5 && _runZ == 0)
            {
                MoveXY(X_CAM, Y_CAM);
                _runX = _runY = 1;
                _step++;

            }
            if (_step == 6 && _runX == 0 && _runY == 0)
            {
                req_CAM = 1;
                capture.Start();
                _step++;
            }
            if (_step == 7 && Cam_check_ok == 0)
            {
                float goclech = Math.Abs(gocquay);

                if (goclech > 2 && goclech < 45)
                    MoveR1(800 + (int)(40 * goclech / 9));
                if (goclech < 90 && goclech > 45)
                {
                    goclech = 90 - goclech;
                    MoveR1(800 - (int)(40 * goclech / 9));
                }

                //if (vX > 1)
                //    MoveXY(_coodinateX - (int)(vX*1.497435), _coodinateY);
                //if (vX < -1)
                //    MoveXY(_coodinateX + (int)(vX * 1.497435), _coodinateY);
                //if (vX < 1 && vX > -1)
                //{
                //    if (vY > 0)
                //        MoveXY(_coodinateX, _coodinateY + (int)(vY * 1.497435));
                //    if (vY < 0)
                //        MoveXY(_coodinateX, _coodinateY - (int)(vY * 1.497435));
                //}

            }
            //====================== DI CHUYEN TOI MAY NAP ROM ===================
            //********************* NOTE *****************************************
            /*
             CHECK TRƯƠC CAM BOTTOM, XOAY => ĐƯA VÀO MÁY NẠP
             * 
             * 
             */
            if (_step == 7 && _runX == 0 && _runY == 0 && Cam_check_ok == 1)  
            {
                Cam_check_ok = 0;
                MoveXY(X_TRAY_ROM[i_in_ROM], Y_TRAY_ROM[i_in_ROM]);
                _runX = _runY = 1;
                _step++;
            }
            if (_step == 8 && _runX == 0 && _runY == 0) // XUONG NOZZLE
            {
                MoveZ(200);
                _runZ = 1;
                // Disable_PUMP();
                Disable_VACUM_1();
                _step++;
            }
            if (_step == 9 && _runZ == 0) // LEN NOZZLE
            {
                MoveZ(5);
                _runZ = 1;

                //_step++;

                i_IC++;
                if (i_IC >= 16)
                    i_IC = 1;

                i_in_ROM++;
                if (i_in_ROM > (int)ROM_Stack_cot.Value * (int)ROM_Stack_hang.Value)
                {
                    //if (i_ROM >= 5)
                    i_in_ROM = 1;
                    Full_Stack_Rom = 1;
                }
                _step = 1;

            }
        }
        private void Disable_VACUM_2()
        {
            // throw new NotImplementedException();
            try
            {
                MB.WriteSingleRegister(ADD_VACUM2, VACUM_2_OFF);
            }
            catch
            {

            }
        }
        private void Enable_VACUM_2()
        {
            // throw new NotImplementedException();
            try
            {
                MB.WriteSingleRegister(ADD_VACUM2, VACUM_2_ON);
            }
            catch
            {

            }
        }
        private void Disable_VACUM_1()
        {
            // throw new NotImplementedException();
            try
            {
                MB.WriteSingleRegister(ADD_VACUM1, VACUM_1_OFF);
            }
            catch
            {

            }
        }
        private void Enable_VACUM_1()
        {
            // throw new NotImplementedException();
            try
            {
                MB.WriteSingleRegister(ADD_VACUM1, VACUM_1_ON);
            }
            catch
            {

            }
        }

        private void Disable_PUMP()
        {
            //throw new NotImplementedException();
            try
            {
                MB.WriteSingleRegister(ADD_PUMP, PUMP_OFF);
            }
            catch
            {

            }
        }

        private void Enable_PUMP()
        {
            //throw new NotImplementedException();
            try
            {
                MB.WriteSingleRegister(ADD_PUMP, PUMP_ON);
            }
            catch
            {

            }
        }
        private void MoveR1(int cood_R1)
        {
            try
            {
                MB.WriteSingleRegister(ADD_REQ_R1, cood_R1);
            }
            catch
            {

            }
        }
        private void MoveR2(int cood_R2)
        {
            try
            {
                MB.WriteSingleRegister(ADD_REQ_R2, cood_R2);
            }
            catch
            {

            }
        }
        private void MoveZ(int cood_z)
        {
            //throw new NotImplementedException();
            try
            {
                MB.WriteSingleRegister(ADD_REQ_Z, cood_z);
            }
            catch
            {

            }
        }

        private void MoveXY(int cood_x, int cood_y)
        {

            //throw new NotImplementedException();
            try
            {
                MB.WriteSingleRegister(ADD_REQ_X, cood_x);
                MB.WriteSingleRegister(ADD_REQ_Y, cood_y);
            }
            catch
            {

            }
        }

        private void SAVE_Click(object sender, EventArgs e)
        {
            IC_End_choice.Hide();
            IC_Start_choice.Hide();


            byte[] val16 = new byte[2];
            ushort val = 0, k1, k2, k3, k4, k5, k6;

            val = ushort.Parse(IC_START_X.Text.ToString());
            k1 = val;

            val16 = BitConverter.GetBytes(val);
            data[1] = val16[0];
            data[2] = val16[1];

            val = ushort.Parse(IC_START_Y.Text.ToString());
            k2 = val;

            val16 = BitConverter.GetBytes(val);
            data[3] = val16[0];
            data[4] = val16[1];

            val = ushort.Parse(IC_END_X.Text.ToString());
            k3 = val;

            val16 = BitConverter.GetBytes(val);
            data[5] = val16[0];
            data[6] = val16[1];

            val = ushort.Parse(IC_END_Y.Text.ToString());
            k4 = val;

            val16 = BitConverter.GetBytes(val);
            data[7] = val16[0];
            data[8] = val16[1];
            //=============== so hang ======================
            data[9] = (byte)IC_Stack_hang.Value;
            k5 = (ushort)IC_Stack_hang.Value;
            //=============== so cot ========================
            data[10] = (byte)IC_Stack_cot.Value;
            k6 = (ushort)IC_Stack_cot.Value;

            if (k1 > 0 & k2 > 0 & k3 > 0 & k4 > 0 & k5 > 0 & k6 > 0)
            {
                File.WriteAllBytes("IC_Stack.tula", data);
                MessageBox.Show("Save IC Stack complete!");

            }
            else
                MessageBox.Show("No Save");

            Creat_IC_Stack();
        }
        public void Creat_IC_Stack()
        {
            IC_Stack.Controls.Clear();
            IC_Stack.RowStyles.Clear();
            IC_Stack.ColumnStyles.Clear();
            int col_MAX = (int)IC_Stack_cot.Value;
            int row_MAX = (int)IC_Stack_hang.Value;
            IC_Stack.ColumnCount = col_MAX;
            IC_Stack.RowCount = row_MAX;
            IC_Stack.Width = col_MAX * 25;
            IC_Stack.Height = row_MAX * 25;


            for (int i = 1; i <= col_MAX * row_MAX; i++)
            {

                Label IC_but = new Label();
                IC_but.Margin = new Padding(1);
                IC_but.Height = 20;
                IC_but.Width = 20;
                IC_but.FlatStyle = FlatStyle.Flat;
                IC_but.TextAlign = ContentAlignment.MiddleCenter;
                IC_but.BorderStyle = BorderStyle.FixedSingle;
                IC_but.Font = new Font("Arial", 6);
                IC_but.BackColor = Color.Green;
                IC_but.Text = i.ToString();
                IC_but.Name = i.ToString();
                IC_Stack.Controls.Add(IC_but, (i - 1) % (col_MAX), (i - 1) / col_MAX);
                IC_but.DoubleClick += IC_but_DoubleClick;
                IC_but.MouseHover += IC_but_MouseHover;
                //IC_but.MouseMove += IC_but_MouseMove;
            }
            //------------------ TINH TOA DO CAC STACK ------------------------------
            X_START_IC = ushort.Parse(IC_START_X.Text.ToString());
            Y_START_IC = ushort.Parse(IC_START_Y.Text.ToString());
            X_END_IC = ushort.Parse(IC_END_X.Text.ToString());
            Y_END_IC = ushort.Parse(IC_END_Y.Text.ToString());
            //socot_IC = (ushort)IC_Stack_cot.Value; ;
            //sohang_IC = (ushort)IC_Stack_hang.Value;
            double Step_X_IC;
            double Step_Y_IC;
            if (col_MAX != 1)
                Step_X_IC = Math.Abs(X_START_IC - X_END_IC) / ((int)IC_Stack_cot.Value - 1);
            else
                Step_X_IC = 0;
            if (row_MAX != 1)
                Step_Y_IC = Math.Abs(Y_START_IC - Y_END_IC) / ((int)IC_Stack_hang.Value - 1);
            else
                Step_Y_IC = 0;
            //MessageBox.Show("Xstep" + Step_X_IC.ToString() + "Y" + Step_Y_IC.ToString());
            int k_x = 0, k_y = 0;

            for (int n = 1; n <= col_MAX * row_MAX; n++)
            {
                //======== X 
                if ((n % col_MAX) == 0)
                {
                    k_x = col_MAX;
                    k_y = 1;
                }
                else
                {
                    k_x = 0;
                    k_y = 0;
                }
                if (X_START_IC <= X_END_IC)
                    X_TRAY_IC[n] = (int)(X_START_IC + ((n % col_MAX + k_x - 1) * Step_X_IC));
                else
                    X_TRAY_IC[n] = (int)(X_START_IC - ((n % col_MAX + k_x - 1) * Step_X_IC));
                // ======== Y

                if (Y_START_IC <= Y_END_IC)
                    Y_TRAY_IC[n] = (int)(Y_START_IC + ((n / col_MAX - k_y) * Step_Y_IC));
                else
                    Y_TRAY_IC[n] = (int)(Y_START_IC - ((n / col_MAX - k_y) * Step_Y_IC));

            }
            //MessageBox.Show((Y_TRAY_IC[col_MAX * row_MAX]).ToString());
            //MessageBox.Show((X_TRAY_IC[col_MAX * row_MAX]).ToString());
            //MessageBox.Show((X_TRAY_IC[1]).ToString());
        }

        void IC_but_MouseMove(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            Label BT = (Label)sender;
            toolTip1.SetToolTip(BT, "[" + X_TRAY_IC[ushort.Parse(BT.Name)].ToString() + " , " + Y_TRAY_IC[ushort.Parse(BT.Name)].ToString() + "]");
            //toolTip1.SetToolTip(BT,"a");
        }

        void IC_but_MouseHover(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Label BT = (Label)sender;
            toolTip1.SetToolTip(BT, "[" + X_TRAY_IC[ushort.Parse(BT.Name)].ToString() + " , " + Y_TRAY_IC[ushort.Parse(BT.Name)].ToString() + "]");
            //toolTip1.SetToolTip(BT,"a");
        }

        void IC_but_DoubleClick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Label lab = (Label)sender;
            if (lab.BackColor == Color.Green)
                lab.BackColor = Color.White;
            else
                lab.BackColor = Color.Green;

        }

        private void SAVE_XY_ROM_Click(object sender, EventArgs e)
        {
            ROM_End_choice.Hide();
            ROM_Start_choice.Hide();

            byte[] val16 = new byte[2];
            ushort val = 0, k1, k2, k3, k4, k5, k6;

            val = ushort.Parse(ROM_START_X.Text.ToString());
            k1 = val;
            val16 = BitConverter.GetBytes(val);
            data[1] = val16[0];
            data[2] = val16[1];

            val = ushort.Parse(ROM_START_Y.Text.ToString());
            k2 = val;
            val16 = BitConverter.GetBytes(val);
            data[3] = val16[0];
            data[4] = val16[1];

            val = ushort.Parse(ROM_END_X.Text.ToString());
            k3 = val;
            val16 = BitConverter.GetBytes(val);
            data[5] = val16[0];
            data[6] = val16[1];

            val = ushort.Parse(ROM_END_Y.Text.ToString());
            k4 = val;
            val16 = BitConverter.GetBytes(val);
            data[7] = val16[0];
            data[8] = val16[1];
            //=============== so hang ======================
            data[9] = (byte)ROM_Stack_hang.Value;
            k5 = (ushort)ROM_Stack_hang.Value;
            //=============== so cot ========================
            data[10] = (byte)ROM_Stack_cot.Value;
            k6 = (ushort)ROM_Stack_cot.Value;
            if (k1 > 0 & k2 > 0 & k3 > 0 & k4 > 0 & k5 > 0 & k6 > 0)
            {
                File.WriteAllBytes("ROM_Stack.tula", data);
                MessageBox.Show("Save ROM Stack complete!");

            }
            else
                MessageBox.Show("No Save");
            Creat_ROM_Stack();

        }
        public void Creat_ROM_Stack()
        {
            ROM_Stack.Controls.Clear();
            ROM_Stack.RowStyles.Clear();
            ROM_Stack.ColumnStyles.Clear();
            int col_MAX = (int)ROM_Stack_cot.Value;
            int row_MAX = (int)ROM_Stack_hang.Value;
            ROM_Stack.ColumnCount = col_MAX;
            ROM_Stack.RowCount = row_MAX;
            ROM_Stack.Width = col_MAX * 40;
            ROM_Stack.Height = row_MAX * 40;


            for (int i = 1; i <= col_MAX * row_MAX; i++)
            {

                Label ROM_but = new Label();
                ROM_but.Margin = new Padding(1);
                ROM_but.Height = 35;
                ROM_but.Width = 35;
                ROM_but.FlatStyle = FlatStyle.Flat;
                ROM_but.TextAlign = ContentAlignment.MiddleCenter;
                ROM_but.BorderStyle = BorderStyle.FixedSingle;
                ROM_but.BackColor = Color.Green;
                ROM_but.Text = i.ToString();
                ROM_but.Name = i.ToString();
                ROM_Stack.Controls.Add(ROM_but, (i - 1) % (col_MAX), (i - 1) / col_MAX);
                ROM_but.DoubleClick += ROM_but_DoubleClick;
                ROM_but.MouseHover += ROM_but_MouseHover;


            }
            //------------------ TINH TOA DO CAC STACK ------------------------------
            X_START_ROM = ushort.Parse(ROM_START_X.Text.ToString());
            Y_START_ROM = ushort.Parse(ROM_START_Y.Text.ToString());
            X_END_ROM = ushort.Parse(ROM_END_X.Text.ToString());
            Y_END_ROM = ushort.Parse(ROM_END_Y.Text.ToString());
            //socot_IC = (ushort)IC_Stack_cot.Value; ;
            //sohang_IC = (ushort)IC_Stack_hang.Value;
            double Step_X_ROM;
            double Step_Y_ROM;
            if (col_MAX != 1)
                Step_X_ROM = Math.Abs(X_START_ROM - X_END_ROM) / ((int)ROM_Stack_cot.Value - 1);
            else
                Step_X_ROM = 0;
            if (row_MAX != 1)
                Step_Y_ROM = Math.Abs(Y_START_ROM - Y_END_ROM) / ((int)ROM_Stack_hang.Value - 1);
            else
                Step_Y_ROM = 0;
            //MessageBox.Show("Xstep" + Step_X_IC.ToString() + "Y" + Step_Y_IC.ToString());
            int k_x = 0, k_y = 0;

            for (int n = 1; n <= col_MAX * row_MAX; n++)
            {
                //======== X 
                if ((n % col_MAX) == 0)
                {
                    k_x = col_MAX;
                    k_y = 1;
                }
                else
                {
                    k_x = 0;
                    k_y = 0;
                }
                if (X_START_ROM <= X_END_ROM)
                    X_TRAY_ROM[n] = (int)(X_START_ROM + ((n % col_MAX + k_x - 1) * Step_X_ROM));
                else
                    X_TRAY_ROM[n] = (int)(X_START_ROM - ((n % col_MAX + k_x - 1) * Step_X_ROM));
                // ======== Y

                if (Y_START_ROM <= Y_END_ROM)
                    Y_TRAY_ROM[n] = (int)(Y_START_ROM + ((n / col_MAX - k_y) * Step_Y_ROM));
                else
                    Y_TRAY_ROM[n] = (int)(Y_START_ROM - ((n / col_MAX - k_y) * Step_Y_ROM));

            }
        }

        void ROM_but_MouseHover(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Label ROM = (Label)sender;
            toolTip1.SetToolTip(ROM, "[" + X_TRAY_ROM[ushort.Parse(ROM.Name)].ToString() + " , " + Y_TRAY_ROM[ushort.Parse(ROM.Name)].ToString() + "]");
        }

        void ROM_but_DoubleClick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Label lab = (Label)sender;
            if (lab.BackColor == Color.Green)
                lab.BackColor = Color.White;
            else
                lab.BackColor = Color.Green;
        }
        private void SET_XY_ROM_Click(object sender, EventArgs e)
        {

            if (ROM_Stack_choice == ROM_START_CHOICE)
            {
                ROM_START_X.Text = Toado_X.Text;
                ROM_START_Y.Text = Toado_Y.Text;
            }
            if (ROM_Stack_choice == ROM_END_CHOICE)
            {
                ROM_END_X.Text = Toado_X.Text;
                ROM_END_Y.Text = Toado_Y.Text;
            }
        }

        private void PUMP_Click(object sender, EventArgs e)
        {
            if (_Status_PUMP == PUMP_OFF)
            {
                _Status_PUMP = PUMP_ON;
                PUMP.BackColor = Color.Green;
            }
            else
            {
                _Status_PUMP = PUMP_OFF;
                PUMP.BackColor = Color.Red;
            }
            try
            {
                MB.WriteSingleRegister(19, _Status_PUMP);
            }
            catch
            {

            }
        }

        private void VACUM_1_Click(object sender, EventArgs e)
        {
            if (_Status_VACUM_1 == VACUM_1_OFF)
            {
                _Status_VACUM_1 = VACUM_1_ON;
                VACUM_1.BackColor = Color.Green;
            }
            else
            {
                _Status_VACUM_1 = VACUM_1_OFF;
                VACUM_1.BackColor = Color.Red;
            }
            try
            {
                MB.WriteSingleRegister(20, _Status_VACUM_1);
            }
            catch
            {

            }

        }

        private void VACUM_2_Click(object sender, EventArgs e)
        {
            if (_Status_VACUM_2 == VACUM_2_OFF)
            {
                _Status_VACUM_2 = VACUM_2_ON;
                VACUM_2.BackColor = Color.Green;
            }
            else
            {
                _Status_VACUM_2 = VACUM_2_OFF;
                VACUM_2.BackColor = Color.Red;
            }
            try
            {
                MB.WriteSingleRegister(21, _Status_VACUM_2);
            }
            catch
            {

            }
        }

        private void LED_Click(object sender, EventArgs e)
        {
            if (_Status_LED == LED_OFF)
            {
                _Status_LED = LED_ON;
                LED.BackColor = Color.Green;
            }
            else
            {
                _Status_LED = LED_OFF;
                LED.BackColor = Color.Red;
            }
            try
            {
                MB.WriteSingleRegister(_Add_LED, _Status_LED);
            }
            catch
            {

            }
        }




        void NG_but_DoubleClick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Label lab = (Label)sender;
            if (lab.BackColor == Color.White)
                lab.BackColor = Color.Red;
            else
            {
                lab.BackColor = Color.White;
            }
        }

        private void Save_NG_Stack_Click(object sender, EventArgs e)
        {
            NG_End_choice.Hide();
            NG_Start_choice.Hide();

            byte[] val16 = new byte[2];
            ushort val = 0, k1, k2, k3, k4, k5, k6;

            val = ushort.Parse(NG_START_X.Text.ToString());
            k1 = val;
            val16 = BitConverter.GetBytes(val);
            data[1] = val16[0];
            data[2] = val16[1];

            val = ushort.Parse(NG_START_Y.Text.ToString());
            k2 = val;
            val16 = BitConverter.GetBytes(val);
            data[3] = val16[0];
            data[4] = val16[1];

            val = ushort.Parse(NG_END_X.Text.ToString());
            k3 = val;
            val16 = BitConverter.GetBytes(val);
            data[5] = val16[0];
            data[6] = val16[1];

            val = ushort.Parse(NG_END_Y.Text.ToString());
            k4 = val;
            val16 = BitConverter.GetBytes(val);
            data[7] = val16[0];
            data[8] = val16[1];
            //=============== so hang ======================
            data[9] = (byte)NG_Stack_hang.Value;
            k5 = (ushort)NG_Stack_hang.Value;
            //=============== so cot ========================
            data[10] = (byte)NG_Stack_cot.Value;
            k6 = (ushort)NG_Stack_cot.Value;
            if (k1 > 0 & k2 > 0 & k3 > 0 & k4 > 0 & k5 > 0 & k6 > 0)
            {
                File.WriteAllBytes("NG_Stack.tula", data);
                MessageBox.Show("Save NG Stack complete!");

            }
            else
                MessageBox.Show("No Save");
            Creat_NG_Stack();
        }
        public void Creat_NG_Stack()
        {
            NG_Stack.Controls.Clear();
            NG_Stack.RowStyles.Clear();
            NG_Stack.ColumnStyles.Clear();
            int col_MAX = (int)NG_Stack_cot.Value;
            int row_MAX = (int)NG_Stack_hang.Value;
            NG_Stack.ColumnCount = col_MAX;
            NG_Stack.RowCount = row_MAX;
            NG_Stack.Width = col_MAX * 25;
            NG_Stack.Height = row_MAX * 25;


            for (int i = 1; i <= col_MAX * row_MAX; i++)
            {

                Label NG_but = new Label();
                NG_but.Margin = new Padding(1);
                NG_but.Height = 20;
                NG_but.Width = 20;
                NG_but.FlatStyle = FlatStyle.Flat;
                NG_but.TextAlign = ContentAlignment.MiddleCenter;
                //NG_but.BorderStyle = BorderStyle.FixedSingle;
                NG_but.BackColor = Color.White;
                NG_but.Text = i.ToString();
                NG_but.Name = i.ToString();
                NG_Stack.Controls.Add(NG_but, (i - 1) % (col_MAX), (i - 1) / col_MAX);
                NG_but.DoubleClick += NG_but_DoubleClick;
                NG_but.MouseHover += NG_but_MouseHover;

            }
            //------------------ TINH TOA DO CAC STACK ------------------------------
            X_START_NG = ushort.Parse(NG_START_X.Text.ToString());
            Y_START_NG = ushort.Parse(NG_START_Y.Text.ToString());
            X_END_NG = ushort.Parse(NG_END_X.Text.ToString());
            Y_END_NG = ushort.Parse(NG_END_Y.Text.ToString());
            //socot_IC = (ushort)IC_Stack_cot.Value; ;
            //sohang_IC = (ushort)IC_Stack_hang.Value;
            double Step_X_NG;
            double Step_Y_NG;
            if (col_MAX != 1)
                Step_X_NG = Math.Abs(X_START_NG - X_END_NG) / ((int)NG_Stack_cot.Value - 1);
            else
                Step_X_NG = 0;
            if (row_MAX != 1)
                Step_Y_NG = Math.Abs(Y_START_NG - Y_END_NG) / ((int)NG_Stack_hang.Value - 1);
            else
                Step_Y_NG = 0;
            //MessageBox.Show("Xstep" + Step_X_IC.ToString() + "Y" + Step_Y_IC.ToString());
            int k_x = 0, k_y = 0;

            for (int n = 1; n <= col_MAX * row_MAX; n++)
            {
                //======== X 
                if ((n % col_MAX) == 0)
                {
                    k_x = col_MAX;
                    k_y = 1;
                }
                else
                {
                    k_x = 0;
                    k_y = 0;
                }
                if (X_START_NG <= X_END_NG)
                    X_TRAY_NG[n] = (int)(X_START_NG + ((n % col_MAX + k_x - 1) * Step_X_NG));
                else
                    X_TRAY_NG[n] = (int)(X_START_NG - ((n % col_MAX + k_x - 1) * Step_X_NG));
                // ======== Y

                if (Y_START_NG <= Y_END_NG)
                    Y_TRAY_NG[n] = (int)(Y_START_NG + ((n / col_MAX - k_y) * Step_Y_NG));
                else
                    Y_TRAY_NG[n] = (int)(Y_START_NG - ((n / col_MAX - k_y) * Step_Y_NG));

            }
        }

        void NG_but_MouseHover(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Label NG = (Label)sender;
            toolTip1.SetToolTip(NG, "[" + X_TRAY_NG[ushort.Parse(NG.Name)].ToString() + " , " + Y_TRAY_NG[ushort.Parse(NG.Name)].ToString() + "]");

        }
        private void Save_Pass_Stack_Click(object sender, EventArgs e)
        {
            PASS_End_choice.Hide();
            PASS_Start_choice.Hide();

            byte[] val16 = new byte[2];
            ushort val = 0, k1, k2, k3, k4, k5, k6;

            val = ushort.Parse(PASS_START_X.Text.ToString());
            k1 = val;
            val16 = BitConverter.GetBytes(val);
            data[1] = val16[0];
            data[2] = val16[1];

            val = ushort.Parse(PASS_START_Y.Text.ToString());
            k2 = val;
            val16 = BitConverter.GetBytes(val);
            data[3] = val16[0];
            data[4] = val16[1];

            val = ushort.Parse(PASS_END_X.Text.ToString());
            k3 = val;
            val16 = BitConverter.GetBytes(val);
            data[5] = val16[0];
            data[6] = val16[1];

            val = ushort.Parse(PASS_END_Y.Text.ToString());
            k4 = val;
            val16 = BitConverter.GetBytes(val);
            data[7] = val16[0];
            data[8] = val16[1];
            //=============== so hang ======================
            data[9] = (byte)PASS_Stack_hang.Value;
            k5 = (ushort)PASS_Stack_hang.Value;
            //=============== so cot ========================
            data[10] = (byte)PASS_Stack_cot.Value;
            k6 = (ushort)PASS_Stack_cot.Value;
            if (k1 > 0 & k2 > 0 & k3 > 0 & k4 > 0 & k5 > 0 & k6 > 0)
            {
                File.WriteAllBytes("PASS_Stack.tula", data);
                MessageBox.Show("Save PASS Stack complete!");

            }
            else
                MessageBox.Show("No Save");
            Creat_Pass_Stack();
        }

        public void Creat_Pass_Stack()
        {
            Pass_Stack.Controls.Clear();
            Pass_Stack.RowStyles.Clear();
            Pass_Stack.ColumnStyles.Clear();
            int col_MAX = (int)PASS_Stack_cot.Value;
            int row_MAX = (int)PASS_Stack_hang.Value;
            Pass_Stack.ColumnCount = col_MAX;
            Pass_Stack.RowCount = row_MAX;
            Pass_Stack.Width = col_MAX * 25;
            Pass_Stack.Height = row_MAX * 25;


            for (int i = 1; i <= col_MAX * row_MAX; i++)
            {

                Label Pass_but = new Label();
                Pass_but.Margin = new Padding(1);
                Pass_but.Height = 20;
                Pass_but.Width = 20;
                Pass_but.FlatStyle = FlatStyle.Flat;
                Pass_but.TextAlign = ContentAlignment.MiddleCenter;
                //Pass_but.BorderStyle = BorderStyle.FixedSingle;
                Pass_but.BackColor = Color.White;
                Pass_but.Text = i.ToString();
                Pass_but.Name = i.ToString();
                Pass_Stack.Controls.Add(Pass_but, (i - 1) % (col_MAX), (i - 1) / col_MAX);
                Pass_but.DoubleClick += Pass_but_DoubleClick;
                Pass_but.MouseHover += Pass_but_MouseHover;

            }
            //------------------ TINH TOA DO CAC STACK ------------------------------
            X_START_PASS = ushort.Parse(PASS_START_X.Text.ToString());
            Y_START_PASS = ushort.Parse(PASS_START_Y.Text.ToString());
            X_END_PASS = ushort.Parse(PASS_END_X.Text.ToString());
            Y_END_PASS = ushort.Parse(PASS_END_Y.Text.ToString());
            //socot_IC = (ushort)IC_Stack_cot.Value; ;
            //sohang_IC = (ushort)IC_Stack_hang.Value;
            double Step_X_PASS;
            double Step_Y_PASS;
            if (col_MAX != 1)
                Step_X_PASS = Math.Abs(X_START_PASS - X_END_PASS) / ((int)PASS_Stack_cot.Value - 1);
            else
                Step_X_PASS = 0;
            if (row_MAX != 1)
                Step_Y_PASS = Math.Abs(Y_START_PASS - Y_END_PASS) / ((int)PASS_Stack_hang.Value - 1);
            else
                Step_Y_PASS = 0;
            //MessageBox.Show("Xstep" + Step_X_IC.ToString() + "Y" + Step_Y_IC.ToString());
            int k_x = 0, k_y = 0;

            for (int n = 1; n <= col_MAX * row_MAX; n++)
            {
                //======== X 
                if ((n % col_MAX) == 0)
                {
                    k_x = col_MAX;
                    k_y = 1;
                }
                else
                {
                    k_x = 0;
                    k_y = 0;
                }
                if (X_START_PASS <= X_END_PASS)
                    X_TRAY_PASS[n] = (int)(X_START_PASS + ((n % col_MAX + k_x - 1) * Step_X_PASS));
                else
                    X_TRAY_PASS[n] = (int)(X_START_PASS - ((n % col_MAX + k_x - 1) * Step_X_PASS));
                // ======== Y

                if (Y_START_PASS <= Y_END_PASS)
                    Y_TRAY_PASS[n] = (int)(Y_START_PASS + ((n / col_MAX - k_y) * Step_Y_PASS));
                else
                    Y_TRAY_PASS[n] = (int)(Y_START_PASS - ((n / col_MAX - k_y) * Step_Y_PASS));

            }

        }

        void Pass_but_MouseHover(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Label PASS = (Label)sender;
            toolTip1.SetToolTip(PASS, "[" + X_TRAY_PASS[ushort.Parse(PASS.Name)].ToString() + " , " + Y_TRAY_PASS[ushort.Parse(PASS.Name)].ToString() + "]");


        }

        void Pass_but_DoubleClick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Label lab = (Label)sender;
            if (lab.BackColor == Color.White)
                lab.BackColor = Color.Green;
            else
                lab.BackColor = Color.White;
        }

        private void IC_Stack_Start_Click(object sender, EventArgs e)
        {
            IC_Start_choice.Show();
            IC_End_choice.Hide();
            IC_Stack_choice = IC_START_CHOICE;
            //MessageBox.Show(IC_Stack_choice.ToString());
        }
        private void IC_Stack_End_Click(object sender, EventArgs e)
        {
            IC_Start_choice.Hide();
            IC_End_choice.Show();
            IC_Stack_choice = IC_END_CHOICE;
            //MessageBox.Show(IC_Stack_choice.ToString());
        }
        private void SET_XY_IC_Click(object sender, EventArgs e)
        {
            if (IC_Stack_choice == IC_START_CHOICE)
            {
                IC_START_X.Text = Toado_X.Text;
                IC_START_Y.Text = Toado_Y.Text;
            }
            if (IC_Stack_choice == IC_END_CHOICE)
            {
                IC_END_X.Text = Toado_X.Text;
                IC_END_Y.Text = Toado_Y.Text;
            }
        }

        private void Recall_Click(object sender, EventArgs e)
        {
            byte[] val16 = new byte[2];
            ushort val = 0;
            #region Data process
            //=============================  DATA IC_STACK ========================================//
            byte[] IC_data = File.ReadAllBytes("IC_Stack.tula");

            //----------------------------- X START --------------------------------//
            val16[0] = IC_data[1];
            val16[1] = IC_data[2];
            val = BitConverter.ToUInt16(val16, 0);
            IC_START_X.Text = val.ToString();
            //---------------------------- Y START -----------------------------------//
            val16[0] = IC_data[3];
            val16[1] = IC_data[4];
            val = BitConverter.ToUInt16(val16, 0);
            IC_START_Y.Text = val.ToString();
            //---------------------------- X END -----------------------------------//
            val16[0] = IC_data[5];
            val16[1] = IC_data[6];
            val = BitConverter.ToUInt16(val16, 0);
            IC_END_X.Text = val.ToString();
            //---------------------------- Y END -----------------------------------//
            val16[0] = IC_data[7];
            val16[1] = IC_data[8];
            val = BitConverter.ToUInt16(val16, 0);
            IC_END_Y.Text = val.ToString();
            //---------------------------- HANG ------------------------------------//
            IC_Stack_hang.Value = (int)IC_data[9];
            //---------------------------- COT -------------------------------------//
            IC_Stack_cot.Value = (int)IC_data[10];
            //---------------------------- TAO MA TRAN -----------------------------//
            Creat_IC_Stack();

            //==================================== DATA ROM STACK ===================================================//
            byte[] ROM_data = File.ReadAllBytes("ROM_Stack.tula");

            //----------------------------- X START --------------------------------//
            val16[0] = ROM_data[1];
            val16[1] = ROM_data[2];
            val = BitConverter.ToUInt16(val16, 0);
            ROM_START_X.Text = val.ToString();
            //---------------------------- Y START -----------------------------------//
            val16[0] = ROM_data[3];
            val16[1] = ROM_data[4];
            val = BitConverter.ToUInt16(val16, 0);
            ROM_START_Y.Text = val.ToString();
            //---------------------------- X END -----------------------------------//
            val16[0] = ROM_data[5];
            val16[1] = ROM_data[6];
            val = BitConverter.ToUInt16(val16, 0);
            ROM_END_X.Text = val.ToString();
            //---------------------------- Y END -----------------------------------//
            val16[0] = ROM_data[7];
            val16[1] = ROM_data[8];
            val = BitConverter.ToUInt16(val16, 0);
            ROM_END_Y.Text = val.ToString();
            //---------------------------- HANG ------------------------------------//
            ROM_Stack_hang.Value = (int)ROM_data[9];
            //---------------------------- COT -------------------------------------//
            ROM_Stack_cot.Value = (int)ROM_data[10];
            //---------------------------- TAO MA TRAN -----------------------------//
            Creat_ROM_Stack();
            //==================================== DATA NG STACK ===================================================//
            byte[] NG_data = File.ReadAllBytes("NG_Stack.tula");

            //----------------------------- X START --------------------------------//
            val16[0] = NG_data[1];
            val16[1] = NG_data[2];
            val = BitConverter.ToUInt16(val16, 0);
            NG_START_X.Text = val.ToString();
            //---------------------------- Y START -----------------------------------//
            val16[0] = NG_data[3];
            val16[1] = NG_data[4];
            val = BitConverter.ToUInt16(val16, 0);
            NG_START_Y.Text = val.ToString();
            //---------------------------- X END -----------------------------------//
            val16[0] = NG_data[5];
            val16[1] = NG_data[6];
            val = BitConverter.ToUInt16(val16, 0);
            NG_END_X.Text = val.ToString();
            //---------------------------- Y END -----------------------------------//
            val16[0] = NG_data[7];
            val16[1] = NG_data[8];
            val = BitConverter.ToUInt16(val16, 0);
            NG_END_Y.Text = val.ToString();
            //---------------------------- HANG ------------------------------------//
            NG_Stack_hang.Value = (int)NG_data[9];
            //---------------------------- COT -------------------------------------//
            NG_Stack_cot.Value = (int)NG_data[10];
            //---------------------------- TAO MA TRAN -----------------------------//
            Creat_NG_Stack();

            //=================================== PASS STACK ===========================================//
            byte[] PASS_data = File.ReadAllBytes("PASS_Stack.tula");

            //----------------------------- X START --------------------------------//
            val16[0] = PASS_data[1];
            val16[1] = PASS_data[2];
            val = BitConverter.ToUInt16(val16, 0);
            PASS_START_X.Text = val.ToString();
            //---------------------------- Y START -----------------------------------//
            val16[0] = PASS_data[3];
            val16[1] = PASS_data[4];
            val = BitConverter.ToUInt16(val16, 0);
            PASS_START_Y.Text = val.ToString();
            //---------------------------- X END -----------------------------------//
            val16[0] = PASS_data[5];
            val16[1] = PASS_data[6];
            val = BitConverter.ToUInt16(val16, 0);
            PASS_END_X.Text = val.ToString();
            //---------------------------- Y END -----------------------------------//
            val16[0] = PASS_data[7];
            val16[1] = PASS_data[8];
            val = BitConverter.ToUInt16(val16, 0);
            PASS_END_Y.Text = val.ToString();
            //---------------------------- HANG ------------------------------------//
            PASS_Stack_hang.Value = (int)PASS_data[9];
            //---------------------------- COT -------------------------------------//
            PASS_Stack_cot.Value = (int)PASS_data[10];
            //---------------------------- TAO MA TRAN -----------------------------//
            Creat_Pass_Stack();

            //================================== CAMERA =======================================//
            byte[] CAM_data = File.ReadAllBytes("CAM.tula");

            //----------------------------- X START --------------------------------//
            val16[0] = CAM_data[1];
            val16[1] = CAM_data[2];
            val = BitConverter.ToUInt16(val16, 0);
            CAM_START_X.Text = val.ToString();
            X_CAM = val;
            //---------------------------- Y START -----------------------------------//
            val16[0] = CAM_data[3];
            val16[1] = CAM_data[4];
            val = BitConverter.ToUInt16(val16, 0);
            CAM_START_Y.Text = val.ToString();
            Y_CAM = val;

            //================================== NOZZLE ========================================//
            byte[] NOZZLE_data = File.ReadAllBytes("NOZZLE.tula");

            //----------------------------- X DOWN_IC --------------------------------//
            val16[0] = NOZZLE_data[1];
            val16[1] = NOZZLE_data[2];
            val = BitConverter.ToUInt16(val16, 0);
            DOWN_IC.Text = val.ToString();

            val16[0] = NOZZLE_data[3];
            val16[1] = NOZZLE_data[4];
            val = BitConverter.ToUInt16(val16, 0);
            DOWN_ROM.Text = val.ToString();

            val16[0] = NOZZLE_data[5];
            val16[1] = NOZZLE_data[6];
            val = BitConverter.ToUInt16(val16, 0);
            DOWN_NG.Text = val.ToString();

            val16[0] = NOZZLE_data[7];
            val16[1] = NOZZLE_data[8];
            val = BitConverter.ToUInt16(val16, 0);
            DOWN_OK.Text = val.ToString();
            //========================= line ======================
            #endregion
            Draw_line_worktable();

        }
        void Draw_line_worktable()
        {
           // Graphics d = WorkTable.CreateGraphics();
            Pen PenBlue = new Pen(System.Drawing.Color.Blue, 10);
            Pen PenRed = new Pen(System.Drawing.Color.Red, 10);


            Bitmap tmp = new Bitmap(450, 450);
            Graphics d = Graphics.FromImage(tmp);
            d.DrawLine(PenBlue, 220, 344, 220, 180);
            d.DrawLine(PenBlue, 220, 185, 280, 185);

            d.DrawLine(PenRed, 300, 180, 300, 20);
            d.DrawLine(PenRed, 305, 20, 200, 20);

            d.DrawLine(PenBlue, 320, 180, 320, 20);
            d.DrawLine(PenBlue, 315, 20, 360, 20);

            TULA = MergedBitmaps(TULA, tmp);
            WorkTable.BackgroundImage = TULA;
        }
        private Bitmap MergedBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            Bitmap result = new Bitmap(Math.Max(bmp1.Width, bmp2.Width),
                                       Math.Max(bmp1.Height, bmp2.Height));
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp2, Point.Empty);
                g.DrawImage(bmp1, Point.Empty);
            }
            return result;
        }
        private void ROM_Stack_Start_Click(object sender, EventArgs e)
        {
            ROM_Start_choice.Show();
            ROM_End_choice.Hide();
            ROM_Stack_choice = ROM_START_CHOICE;
        }

        private void ROM_Stack_End_Click(object sender, EventArgs e)
        {
            ROM_Start_choice.Hide();
            ROM_End_choice.Show();
            ROM_Stack_choice = ROM_END_CHOICE;
        }

        private void SET_XY_NG_Click(object sender, EventArgs e)
        {
            if (NG_Stack_choice == NG_START_CHOICE)
            {
                NG_START_X.Text = Toado_X.Text;
                NG_START_Y.Text = Toado_Y.Text;
            }
            if (NG_Stack_choice == NG_END_CHOICE)
            {
                NG_END_X.Text = Toado_X.Text;
                NG_END_Y.Text = Toado_Y.Text;
            }
        }

        private void SET_XY_CAM_Click(object sender, EventArgs e)
        {
            if (CAM_Stack_choice == CAM_START_CHOICE)
            {
                CAM_START_X.Text = Toado_X.Text;
                CAM_START_Y.Text = Toado_Y.Text;
            }

        }

        private void Save_CAM_Stack_Click(object sender, EventArgs e)
        {

            CAM_Start_choice.Hide();

            byte[] val16 = new byte[2];
            ushort val = 0, k1, k2;

            val = ushort.Parse(CAM_START_X.Text.ToString());
            k1 = val;
            val16 = BitConverter.GetBytes(val);
            data[1] = val16[0];
            data[2] = val16[1];

            val = ushort.Parse(CAM_START_Y.Text.ToString());
            k2 = val;
            val16 = BitConverter.GetBytes(val);
            data[3] = val16[0];
            data[4] = val16[1];


            if (k1 > 0 & k2 > 0)
            {
                File.WriteAllBytes("CAM.tula", data);
                MessageBox.Show("Save CAM complete!");

            }
            else
                MessageBox.Show("No Save");
        }

        private void NG_Stack_Start_Click(object sender, EventArgs e)
        {
            NG_Start_choice.Show();
            NG_End_choice.Hide();
            NG_Stack_choice = NG_START_CHOICE;
        }

        private void NG_Stack_End_Click(object sender, EventArgs e)
        {
            NG_Start_choice.Hide();
            NG_End_choice.Show();
            NG_Stack_choice = NG_END_CHOICE;
        }

        private void PASS_Stack_Start_Click(object sender, EventArgs e)
        {
            PASS_Start_choice.Show();
            PASS_End_choice.Hide();
            PASS_Stack_choice = PASS_START_CHOICE;
        }

        private void PASS_Stack_End_Click(object sender, EventArgs e)
        {
            PASS_Start_choice.Hide();
            PASS_End_choice.Show();
            PASS_Stack_choice = PASS_END_CHOICE;
        }

        private void CAM_Stack_Start_Click(object sender, EventArgs e)
        {
            CAM_Start_choice.Show();

            CAM_Stack_choice = CAM_START_CHOICE;
        }

        private void CAM_Stack_End_Click(object sender, EventArgs e)
        {
            CAM_Start_choice.Hide();

            CAM_Stack_choice = CAM_END_CHOICE;
        }

        private void SET_XY_PASS_Click(object sender, EventArgs e)
        {
            if (PASS_Stack_choice == PASS_START_CHOICE)
            {
                PASS_START_X.Text = Toado_X.Text;
                PASS_START_Y.Text = Toado_Y.Text;
            }
            if (PASS_Stack_choice == PASS_END_CHOICE)
            {
                PASS_END_X.Text = Toado_X.Text;
                PASS_END_Y.Text = Toado_Y.Text;
            }
        }

        private void Save_Nozzle_Click(object sender, EventArgs e)
        {
            byte[] val16 = new byte[2];
            ushort val = 0, k1, k2, k3, k4;

            val = ushort.Parse(DOWN_IC.Text.ToString());
            k1 = val;
            val16 = BitConverter.GetBytes(val);
            data[1] = val16[0];
            data[2] = val16[1];

            val = ushort.Parse(DOWN_ROM.Text.ToString());
            k2 = val;
            val16 = BitConverter.GetBytes(val);
            data[3] = val16[0];
            data[4] = val16[1];

            val = ushort.Parse(DOWN_NG.Text.ToString());
            k3 = val;
            val16 = BitConverter.GetBytes(val);
            data[5] = val16[0];
            data[6] = val16[1];

            val = ushort.Parse(DOWN_OK.Text.ToString());
            k4 = val;
            val16 = BitConverter.GetBytes(val);
            data[7] = val16[0];
            data[8] = val16[1];

            if (k1 > 0 & k2 > 0 & k3 > 0 & k4 > 0)
            {
                File.WriteAllBytes("Nozzle.tula", data);
                MessageBox.Show("Save Nozzle complete!");

            }
            else
                MessageBox.Show("No Save");
        }

        private void CONNECT_Click(object sender, EventArgs e)
        {
            CONNECT.Text = "Disconnect";
            MB.Connect();
            timer1.Enabled = true;
        }

        private void Recall_MouseMove(object sender, MouseEventArgs e)
        {
            // toolTip1.SetToolTip(Recall,"show map");
        }

        private void Recall_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(Recall, "show map1");
        }

        private void Start_auto_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("CHAY AUTO");
            // GO HOME
            if (Start == 0)
            {
                WorkTable.Enabled = false;

                Installing.Show();

                Start_auto.Text = "RUN";
                Start_auto.BackColor = Color.LimeGreen;
                Manual.Enabled = false;
                Calib_System.Enabled = false;
                Start = 1;


                _Req_ORG = 1;
                AUTO_RUN = 1;
                _READY_RUN = 0;



            }
            else
            {
                Start_auto.Text = "STOP";
                Start_auto.BackColor = Color.Red;
                Manual.Enabled = true;
                Calib_System.Enabled = true;

                Start = 0;
                AUTO_RUN = 0;
                _READY_RUN = 0;
                Disable_PUMP();

                MB.WriteSingleRegister(ADD_AM, _Manual);
                _AM = _Manual;
                Automanual.Text = "Manual";


            }

        }

        private void Pause_Click(object sender, EventArgs e)
        {
            if (_PAUSE == 0)
            {
                _PAUSE = 1;
                Pause.Text = "Pause";
                Pause.BackColor = Color.Yellow;
                Manual.Enabled = true;
            }
            else
            {
                _PAUSE = 0;
                Pause.Text = "Play";
                Pause.BackColor = Color.LimeGreen;
                Manual.Enabled = false;
            }
        }

        private void Automanual_Click(object sender, EventArgs e)
        {
            if (Automanual.Text == "Auto")
            {
                Automanual.Text = "Manual";
                _AM = _Manual;
 
            }
            else
            {
                Automanual.Text = "Auto";
                _AM = _Auto;

            }
            try
            {
                MB.WriteSingleRegister(_Add_AutoManual, _AM);
            }
            catch
            {

            }
        }

        private void Camera_connect_Click(object sender, EventArgs e)
        {
            //comboBox1.Text = "USB2.0 Grabber";
            MB.WriteSingleRegister(_Add_LED, LED_ON);
            if (capture == null)
            {
                capture = new Emgu.CV.Capture(1);
                //capture.SetCaptureProperty(camera);
                capture.ImageGrabbed += Capture_ImageGrabbed;
                //capture.Start();
                //capture.Pause();
                
            }
        }
        void  Start_CAM()
        {
            MB.WriteSingleRegister(_Add_LED, LED_ON);
            if (capture == null)
            {
                capture = new Emgu.CV.Capture(1);
                //capture.SetCaptureProperty(camera);
                capture.ImageGrabbed += Capture_ImageGrabbed;
                //capture.Start();
                //capture.Pause();

            }
        }
        private void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            try
            {
                Mat m = new Mat();
                capture.Retrieve(m);
                //pictureBox3.Image = m.ToImage<Bgr, byte>().Bitmap;
                imgInput = m.ToImage<Bgr, byte>();
                bounding_box();
            }
            catch// (Exception ex)
            {
              
            }
        }

        private void bounding_box()
        {
            if (CAM_CHOICE == CAM_BOT)
            {
                Image<Gray, byte> imgOutput = imgInput.Convert<Gray, byte>().ThresholdBinary(new Gray(245), new Gray(255));
                Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
                Emgu.CV.Util.VectorOfVectorOfPoint contours_T = new Emgu.CV.Util.VectorOfVectorOfPoint();
                Mat hier = new Mat();

                Image<Gray, byte> imgout = new Image<Gray, byte>(imgInput.Width, imgInput.Height, new Gray(0));

                CvInvoke.FindContours(imgOutput, contours, hier, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

                Dictionary<int, double> dict = new Dictionary<int, double>();

                List<Point> points = new List<Point>();////////
                //Rectangle roi = new Rectangle(Your Top Left X,Your Top Left Y,Your ROI Width,Your ROI Height);

                if (contours.Size > 0)
                {
                    for (int i = 0; i < contours.Size; i++)
                    {
                        double area = CvInvoke.ContourArea(contours[i]);
                        Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);

                        if (rect.Width > 5 && rect.Height > 5)// && area < 30000)
                        {
                            //dict.Add(i, area);
                            points.AddRange(contours[i].ToArray());/////////////////////////////////////////////

                        }
                    }
                }
                //Image<Bgr, byte> imgout1 = new Image<Bgr, byte>(imgInput.Width, imgInput.Height, new Bgr(0, 0, 0));


                ///===============================
                RotatedRect minAreaRect = CvInvoke.MinAreaRect(points.Select(pt => new PointF(pt.X, pt.Y)).ToArray());/////////////
                Point[] vertices = minAreaRect.GetVertices().Select(pt => new Point((int)pt.X, (int)pt.Y)).ToArray();//=//////////////

                gocquay = minAreaRect.Angle;
                Wchip = minAreaRect.Size.Width;

                rong = imgInput.Bitmap.Width;
                dai = imgInput.Bitmap.Height;

                vX = minAreaRect.Center.X - rong / 2;
                vY = minAreaRect.Center.Y - dai / 2;


                imgInput.Draw(vertices, new Bgr(Color.Lime), 3);
            }
                /////
                Graphics g = Graphics.FromImage(imgInput.Bitmap);
                Pen myPen = new Pen(System.Drawing.Color.Red, 3);

                g.DrawLine(myPen, imgInput.Bitmap.Width / 2, 0, imgInput.Bitmap.Width / 2, imgInput.Bitmap.Height);
                g.DrawLine(myPen, 0, imgInput.Bitmap.Height / 2, imgInput.Bitmap.Width, imgInput.Bitmap.Height / 2);
            
            pictureBox3.Image = imgInput.Bitmap;
            solan_chup++;
            if ((gocquay > -2 || gocquay < -89) && solan_chup > 20)// && (Math.Abs(vX)< 2) && (Math.Abs(vY) < 2))
            {
                Cam_check_ok = 1;
       
                capture.Pause();
           
                solan_chup = 0;
            }
            else
                Cam_check_ok = 0;
            
        }

        private void XL1_UP_Click(object sender, EventArgs e)
        {
            if (_Status_XL1 == XL1_UP)
            {
                _Status_XL1 = XL1_DOWN;
                XL1.BackColor = Color.Green;
            }
            else
            {
                _Status_XL1 = XL1_UP;
                XL1.BackColor = Color.Red;
            }
            try
            {
                MB.WriteSingleRegister(_Add_Xilanh, _Status_XL1);
            }
            catch
            {

            }
        }

        private void R1_UP_MouseDown(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 1;
                _coodinate = _R1;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }

        private void R1_UP_MouseUp(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 0;
                _coodinate = _R1;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }

        private void R2_UP_MouseDown(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 1;
                _coodinate = _R2;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }

        private void R2_UP_MouseUp(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 0;
                _coodinate = _R2;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }

        private void R1_DOWN_MouseDown(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 2;
                _coodinate = _R1;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }

        private void R1_DOWN_MouseUp(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 0;
                _coodinate = _R1;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }

        private void R2_DOWN_MouseDown(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 2;
                _coodinate = _R2;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }
            }
        }

        private void R2_DOWN_MouseUp(object sender, MouseEventArgs e)
        {
            if (_AM == _Manual)
            {
                _ups = 0;
                _coodinate = _R2;
                try
                {
                    MB.WriteSingleRegister(_Add_COOD, _coodinate);
                    MB.WriteSingleRegister(_Add_UPS, _ups);
                }
                catch
                {

                }

            }
        }



        private void Nap_Click(object sender, EventArgs e)
        {
            if (Nap_rom == 0)
            {
                Nap_rom = 1;
                Nap.Text = "Dangnap";

            }
            else         
            {
                Nap_rom = 0;
                Nap.Text = "Nap rom";

            }
        }

        private void Rotation_Click(object sender, EventArgs e)
        {
            float goclech = Math.Abs(gocquay);
            if (goclech > 2 && goclech < 45)
                MoveR1(90 + (int)(40*goclech/9));
            if(goclech < 90 && goclech > 45)
                MoveR1(90 - (int)(40 * goclech / 9));
        }

        private void Camera_choice_Click(object sender, EventArgs e)
        {
            //capture.Stop();
            if(CAM_CHOICE == CAM_BOT)
            {
                CAM_CHOICE = CAM_TOP;
                Camera_choice.Text = "CAM T";
            }
            else
            {
                CAM_CHOICE = CAM_BOT;
                Camera_choice.Text = "CAM B";
            }
            MB.WriteSingleRegister(_Add_CAM, CAM_CHOICE);
            //capture.Start();
        }

        private void Speed_choice_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                MB.WriteSingleRegister(_Add_SPEED, int.Parse(Speed_choice.SelectedItem.ToString()));
            }
            catch
            {
                MessageBox.Show("Edit speed er");
            }
                
        }

        private void HOME_Click(object sender, EventArgs e)
        {
            Automanual.Text = "Manual";
            try
            {
                Req_GOHOME();
            }
            catch //(Exception ex)
            {

                MB.Connect();
            }
            _step = 0;
        }








    }

}

