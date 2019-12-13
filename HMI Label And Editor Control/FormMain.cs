using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace How_to_create_HMI_Control_Real_Time
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            InitializeRegisters();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                
                ModbusRTUProtocol.Start();
                displayControl1.Register = ModbusRTUProtocol.Registers[0];
                displayControl2.Register = ModbusRTUProtocol.Registers[1];
                displayControl3.Register = ModbusRTUProtocol.Registers[2];
                displayControl4.Register = ModbusRTUProtocol.Registers[3];
                displayControl5.Register = ModbusRTUProtocol.Registers[4];

                editorControl1.Register = ModbusRTUProtocol.Registers[5];
                editorControl2.Register = ModbusRTUProtocol.Registers[6];
                editorControl3.Register = ModbusRTUProtocol.Registers[7];
                editorControl4.Register = ModbusRTUProtocol.Registers[8];
                editorControl5.Register = ModbusRTUProtocol.Registers[9];
              
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void InitializeRegisters()
        {
            ModbusRTUProtocol.Registers.Clear();
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40001 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40002 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40003 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40004 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40005 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40006 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40007 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40008 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40009 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40010 });
      
        }
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

    }
}
