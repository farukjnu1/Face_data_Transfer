using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using Face_data_Transfer.BO;

namespace Face_data_Transfer
{
    public partial class Form1 : Form
    {
        string strConGPSDB = "Server=localhost;Database=GPSDB;User Id=sa;Password=123";
        string strConAkash_Attd = "Server=localhost;Database=Akash_attd;User Id=sa;Password=123";

        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader rd;

        public Form1()
        {
            InitializeComponent();
        }

        
        private void Save()
        {
            hr_AttLogs hrAttLog = new hr_AttLogs();
            con = new SqlConnection(strConGPSDB);
            cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            cmd.CommandText = "SELECT TOP 1 * FROM [hr_AttLogs] ORDER BY Log_time DESC";
            rd = cmd.ExecuteReader();
            rd.Read();
            if (rd.HasRows)
            {
                hrAttLog.Id = rd.GetInt32(0);
                hrAttLog.EnrollNumber = rd.GetString(1);
                hrAttLog.VerifyMode = rd.GetInt32(2);
                hrAttLog.InOutMode = rd.GetInt32(3);
                hrAttLog.Log_time = rd.GetDateTime(4);
                hrAttLog.WorkCode = rd.GetInt32(5);
            }
            cmd.Dispose();
            con.Close();

            List<CHECKINOUT> checkInOuts = new List<CHECKINOUT>();
            con = new SqlConnection(strConAkash_Attd);
            cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            string query = "SELECT USERINFO.BADGENUMBER,USERINFO.USERID,CHECKINOUT.CHECKTIME,CHECKINOUT.VERIFYCODE,CHECKINOUT.WorkCode"
                + " FROM [CHECKINOUT] INNER JOIN USERINFO on USERINFO.USERID=CHECKINOUT.USERID WHERE CHECKINOUT.CHECKTIME > '" + hrAttLog.Log_time +"'";
            cmd.CommandText = query;
            rd = cmd.ExecuteReader();
            while(rd.Read())
            {
                CHECKINOUT checkInOut = new CHECKINOUT();
                checkInOut.BADGENUMBER = rd.GetString(0);
                checkInOut.USERID = rd.GetInt32(1);
                checkInOut.CHECKTIME = rd.GetDateTime(2);
                checkInOut.VERIFYCODE = rd.GetInt32(3);
                checkInOut.WorkCode = rd.GetString(4);

                checkInOuts.Add(checkInOut);
            }
            cmd.Dispose();
            con.Close();

            con = new SqlConnection(strConGPSDB);
            cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            //Start the transaction    
            SqlTransaction trans;
            trans = con.BeginTransaction();
            cmd.Transaction = trans;
            try
            {
                foreach (var checkInOut in checkInOuts)
                {
                    //Create the SqlCommand object, specifying the transaction through
                    //the constructor (along with the SQL string and SqlConnection)

                    /*INSERT INTO [hr_AttLogs]([EnrollNumber],[VerifyMode],[InOutMode],[Log_time],[WorkCode])
                        VALUES(<EnrollNumber, varchar(50),>,<VerifyMode, int,>,<InOutMode, int,>,<Log_time, datetime,>,<WorkCode, int,>)*/
                    string insert = "INSERT INTO [hr_AttLogs]([EnrollNumber],[VerifyMode],[InOutMode],[Log_time],[WorkCode])"
                        + " VALUES('" + checkInOut.BADGENUMBER + "'," + checkInOut.VERIFYCODE + "," + 0 + ",'" + checkInOut.CHECKTIME + "'," + checkInOut.WorkCode + ")";
                    cmd.CommandText = insert;
                    cmd.ExecuteNonQuery();
                }
                //If we reach here, all command succeeded, so commit the transaction
                trans.Commit();
                MessageBox.Show("Save successfully!");
            }
            catch (Exception ex)
            {
                //Something went wrong, so rollback the transaction
                trans.Rollback();
                //throw//Bubble up the exception
                MessageBox.Show("Error! rollbacked\r\n" + ex.Message);
            }
            finally
            {
                con.Close();	//Finally, close the connection
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }
    }
}
