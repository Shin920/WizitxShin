using Google.Protobuf.WellKnownTypes;
using MetroFramework.Controls;
using spwho1.DAC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace spwho1
{


    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        //클릭 이벤트용 전역변수 추가
        private Session selectedSession = null;
        public Form1()
        {
            InitializeComponent();
            DefaultSet();
        }

        private void DefaultSet()
        {
            //btn Color
            btnKill.UseCustomForeColor = true;
            btnKill.ForeColor = Color.Red;

            //Grid Set
            dgvSp.BackgroundColor = Color.FromArgb(235, 240, 245);
            //dgvSp.BorderStyle = BorderStyle.FixedSingle;

            dgvSp.ColumnHeadersVisible = true;
            dgvSp.RowHeadersVisible = false;

            dgvSp.AllowUserToAddRows = false;
            dgvSp.AllowUserToDeleteRows = false;
            dgvSp.AllowUserToResizeRows = false;

            dgvSp.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSp.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSp.MultiSelect = false;

            tbUrl.Text = "https://github.com/Shin920/WizitERP";
            tbUrl.ReadOnly = true;
            //tbUrl.BorderStyle = BorderStyle.none;
            tbUrl.BackColor = this.BackColor;
            tbUrl.TabStop = false;
        }

        private void DataLoadSession()
        {
            SpDAC all = new SpDAC();
            dgvSp.DataSource = all.GetAll().DefaultView;
            all.Dispose();
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            //데이터 바인딩
            DataLoadSession();
            //색상표시 함수 호출
            HighlightBlockingRows();
        }


        /// <summary>
        /// 블로킹 관계를 분석하여 DataGridView 행 색상을 변경
        /// 빨간색 : 다른 세션을 막고 있는 원인 세션
        /// 흰색 : 일반 세션
        /// </summary>
        private void HighlightBlockingRows()
        {
            // =====================================================
            // 1. 기존 색상 초기화
            // =====================================================
            foreach (DataGridViewRow row in dgvSp.Rows)
            {
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;
            }

            // =====================================================
            // 2. 현재 블로킹을 발생시키고 있는 SPID 목록 수집
            //
            // 예)
            // SPID 62  BlkBy .
            // SPID 71  BlkBy 62
            // SPID 72  BlkBy 62
            //
            // 이 경우 62가 블로킹 원인 세션
            // =====================================================
            HashSet<string> blockingSpids = new HashSet<string>();

            foreach (DataGridViewRow row in dgvSp.Rows)
            {
                string blkBy = Convert.ToString(row.Cells["BlkBy"].Value).Trim();

                // "." 은 블로킹이 없다는 의미
                if (!string.IsNullOrEmpty(blkBy) &&
                    blkBy != ".")
                {
                    blockingSpids.Add(blkBy);
                }
            }

            // =====================================================
            // 3. 수집된 SPID와 일치하는 행을 찾아 색상 변경
            //
            // 예)
            // blockingSpids = { "62" }
            //
            // SPID 62 -> 빨간색 표시
            // =====================================================
            foreach (DataGridViewRow row in dgvSp.Rows)
            {
                string spid = Convert.ToString(row.Cells["SPID"].Value).Trim();

                if (blockingSpids.Contains(spid))
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
            }

            foreach (DataGridViewRow row in dgvSp.Rows)
            {
                string blkBy = Convert.ToString(row.Cells["BlkBy"].Value).Trim();

                // 현재 다른 세션에 의해 블로킹 당한 상태
                if (!string.IsNullOrEmpty(blkBy) &&
                    blkBy != ".")
                {
                    row.DefaultCellStyle.BackColor = Color.Khaki;
                }
            }
        }


        private void dgvSp_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            SetSelectedSession(e.RowIndex);
        }

        private void SetSelectedSession(int rowIndex)
        {
            DataGridViewRow row = dgvSp.Rows[rowIndex];

            selectedSession = new Session
            {
                SpID = Convert.ToInt32(row.Cells["SPID"].Value),
                Status = row.Cells["Status"].Value?.ToString(),
                Hostname = row.Cells["HostName"].Value?.ToString(),
                EmpNo = row.Cells["EmpNo"].Value?.ToString(),
                EmpName = row.Cells["EmpName"].Value?.ToString(),
                Blkby = row.Cells["BlkBy"].Value?.ToString(),
                Command = row.Cells["Command"].Value?.ToString()
            };
        }



        private void btnKill_Click(object sender, EventArgs e)
        {            

            if (selectedSession == null)
            {
                MessageBox.Show("종료할 세션을 선택하세요.");
                return;
            }

            string command = selectedSession.Command?.ToUpper() ?? "";
           
            if (command.Contains("UPDATE") ||
                command.Contains("INSERT") ||
                command.Contains("DELETE") ||
                command.Contains("MERGE"))           
            {
                MessageBox.Show("작업 중인 세션은 종료할 수 없습니다.");
                return;
            }

            DialogResult result = MessageBox.Show(
                $"SPID {selectedSession.SpID} 세션을 종료하시겠습니까?",
                "KILL 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            SpDAC dac = new SpDAC();

            try
            {
                dac.Kill_Process(selectedSession.SpID);
                MessageBox.Show("세션 종료 요청이 완료되었습니다.");

                DataLoadSession();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dac.Dispose();
            }
        }

        private void btnRollback_Click(object sender, EventArgs e)
        {
            string prno = txtPrno.Text.Trim();

            if (prno.Length != 8 || !prno.All(char.IsDigit))
            {
                MessageBox.Show("제조번호는 숫자 8자리로 입력하세요.");
                return;
            }

            DialogResult result = MessageBox.Show(
                $"제조번호 [{prno}] 기준으로 데이터를 초기화하시겠습니까?",
                "확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            using (SpDAC dac = new SpDAC())
            {
                try
                {
                    int rows = dac.ResetPrnoData(prno);
                    MessageBox.Show($"처리 완료되었습니다.\n영향받은 행 수: {rows}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("처리 실패: " + ex.Message);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            string pdno = txtPdno.Text.Trim();

            if (pdno.Length != 11 )
            {
                MessageBox.Show("생산의뢰번호 11자리로 입력하세요.");
                return;
            }

            DialogResult result = MessageBox.Show(
                $"의뢰번호 [{pdno}] 기준으로 데이터를 초기화하시겠습니까?",
                "확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            using (SpDAC dac = new SpDAC())
            {
                try
                {
                    int rows = dac.CancelByPdno(pdno);
                    MessageBox.Show($"처리 완료되었습니다.\n영향받은 행 수: {rows}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("처리 실패: " + ex.Message);
                }
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            string prno = txtPrno2.Text.Trim();

            if (prno.Length != 8 || !prno.All(char.IsDigit))
            {
                MessageBox.Show("제조번호 8자리로 입력하세요.");
                return;
            }

            DialogResult result = MessageBox.Show(prno + "\n를 표시합니까?", "제조번호 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {

                SpDAC sDAC = new SpDAC();
                int rowsAffected = sDAC.Update(prno);
                sDAC.Dispose();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("수정 완료\n안산ERP에서 확인 해주세요", "알림");
                    this.DialogResult = DialogResult.Yes;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("수정 실패.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
