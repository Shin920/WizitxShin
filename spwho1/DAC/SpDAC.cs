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


        // 공정진행표 생성취소 함수 = 트랜잭션
        public int ResetPrnoData(string prno)
        {
            if (string.IsNullOrWhiteSpace(prno) || prno.Length != 8 || !prno.All(char.IsDigit))
                throw new Exception("제조번호는 숫자 8자리여야 합니다.");

            SqlTransaction tran = null;

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                tran = conn.BeginTransaction();

                int affectedRows = 0;

                string sql1 = "DELETE FROM T_PSHDM WHERE prno = @prno";
                using (SqlCommand cmd = new SqlCommand(sql1, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@prno", prno);
                    affectedRows += cmd.ExecuteNonQuery();
                }

                string sql2 = "DELETE FROM T_PSHDD WHERE prno = @prno";
                using (SqlCommand cmd = new SqlCommand(sql2, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@prno", prno);
                    affectedRows += cmd.ExecuteNonQuery();
                }

                string sql3 = @"
            UPDATE T_PRSEQ
               SET PR250C = PR250
             WHERE prno = @prno";

                using (SqlCommand cmd = new SqlCommand(sql3, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@prno", prno);
                    affectedRows += cmd.ExecuteNonQuery();
                }

                tran.Commit();
                return affectedRows;
            }
            catch
            {
                tran?.Rollback();
                throw;
            }
        }


        //의뢰번호로 생성취소
        public int CancelByPdno(string pdno)
        {
            if (string.IsNullOrWhiteSpace(pdno))
                throw new Exception("생산의뢰번호를 입력하세요.");

            SqlTransaction tran = null;

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                tran = conn.BeginTransaction();

                int affectedRows = 0;

                string sql1 = @"
            DELETE FROM T_PSHDM
             WHERE prno IN (
                   SELECT prno
                     FROM T_ORD
                    WHERE pdno = @pdno
             )";

                using (SqlCommand cmd = new SqlCommand(sql1, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@pdno", pdno);
                    affectedRows += cmd.ExecuteNonQuery();
                }

                string sql2 = @"
            DELETE FROM T_PSHDD
             WHERE prno IN (
                   SELECT prno
                     FROM T_ORD
                    WHERE pdno = @pdno
             )";

                using (SqlCommand cmd = new SqlCommand(sql2, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@pdno", pdno);
                    affectedRows += cmd.ExecuteNonQuery();
                }

                string sql3 = @"
            UPDATE T_PRSEQ
               SET PR250C = PR250
             WHERE prno IN (
                   SELECT prno
                     FROM T_ORD
                    WHERE pdno = @pdno
             )";

                using (SqlCommand cmd = new SqlCommand(sql3, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@pdno", pdno);
                    affectedRows += cmd.ExecuteNonQuery();
                }

                tran.Commit();
                return affectedRows;
            }
            catch
            {
                tran?.Rollback();
                throw;
            }
        }

    }
}
