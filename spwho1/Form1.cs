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
        public Form1()
        {
            InitializeComponent();
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
    }
}
