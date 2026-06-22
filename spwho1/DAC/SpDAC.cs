using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace spwho1.DAC
{
    public class Session
    {
        public int SpID { get; set; }

        public string Status { get; set; }

        public string Hostname { get; set; }

        public string EmpNo { get; set; }

        public string EmpName { get; set; }
        
        public string Blkby { get; set; }

        public string Command { get; set; }
    }

    class SpDAC : IDisposable
    {
        SqlConnection conn;

        public SpDAC()
        {
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            conn.Open();
        }
        public void Dispose()
        {
            conn.Close();
        }


        public DataTable GetAll()
        {
            DataTable dt = new DataTable();
            StringBuilder sql = new StringBuilder();
            sql.Append("CREATE TABLE #sp_who2\r\n(\r\n    " +
                "SPID INT,\r\n    Status VARCHAR(255),\r\n    " +
                "Login VARCHAR(255),\r\n    HostName VARCHAR(255),\r\n    " +
                "BlkBy VARCHAR(255),\r\n    DBName VARCHAR(255),\r\n    " +
                "Command VARCHAR(255),\r\n    CPUTime INT,\r\n    DiskIO INT,\r\n    " +
                "LastBatch VARCHAR(255),\r\n    ProgramName VARCHAR(255),\r\n    " +
                "SPID2 INT\r\n)\r\n\r\nINSERT INTO #sp_who2\r\nEXEC sp_who2\r\n\r\nSELECT\r\n    " +
                "w.SPID,\r\n    w.Status,\r\n    w.HostName,\r\n    RIGHT(RTRIM(w.HostName), 5) AS EmpNo,\r\n    " +
                "h.psknm AS EmpName,\r\n    w.BlkBy,\r\n    w.Command\r\nFROM #sp_who2 w\r\nLEFT JOIN HI230 h\r\n    " +
                "ON h.pscd = RIGHT(RTRIM(w.HostName), 5)\r\nWHERE w.BlkBy <> '.'\r\n\r\nDROP TABLE #sp_who2");

            using (SqlCommand cmd = new SqlCommand(sql.ToString(), conn))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        public int Kill_Process(int spID)
        {
            try
            {
                string sql = $"KILL {spID}";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("KILL 실패 : " + ex.Message);
            }
        }


    }
}
