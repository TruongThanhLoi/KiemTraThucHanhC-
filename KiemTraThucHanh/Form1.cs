using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KiemTraThucHanh
{
    public partial class frmQLNV : Form
    {
        QLNVDataContext db;
        public frmQLNV()
        {
            InitializeComponent();
            LoadTreeView();
            AnTextBoxPhong(true);
            AnTextBoxNV(true);
            txtMaNV.Enabled = false;
            btnLuu.Enabled = false;
            btnSua.Enabled = false;
        }

        private void frmQLNV_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dg;
            dg = MessageBox.Show("Bạn có chắc chắn thoát?", "Thoát",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1);
            if (dg == DialogResult.No)
                e.Cancel = true;
        }
        private void AnTextBoxPhong(bool b)
        {
            txtMaPhong.Enabled = !b;
            txtTenPhong.Enabled = !b;
        }
        private void AnTextBoxNV(bool b)
        {
            txtHoTen.Enabled = !b;
            txtNamSinh.Enabled = !b;
            txtChucVu.Enabled = !b;
            txtLuong.Enabled = !b;
        }
        private void LoadTreeView()
        {
            db = new QLNVDataContext();
            var lstPhong = db.PhongBans.ToList();
            TreeNode node;
            treDanhSachPhong.Nodes.Clear();
            if (lstPhong != null)
            {
                foreach(var i in lstPhong)
                {
                    node = new TreeNode(i.tenPhong);
                    node.Tag = i.maPhong;
                    treDanhSachPhong.Nodes.Add(node);
                }
                treDanhSachPhong.ExpandAll();
            }
        }

        private void treDanhSachPhong_AfterSelect(object sender, TreeViewEventArgs e)
        {
            LoadGridView((int)e.Node.Tag);
            FormatGridView();
        }
        private void LoadGridView(int maphong)
        {
            db = new QLNVDataContext();
            var lstNhanVien = db.NhanViens.Where(n => n.maPhong == maphong).ToList();
            dgvDanhSachNV.DataSource = lstNhanVien;
        }
        private void FormatGridView()
        {
            if (dgvDanhSachNV.Columns["maNhanVien"] != null)
            {
                dgvDanhSachNV.Columns["maNhanVien"].HeaderText = "Mã nhân viên";
                dgvDanhSachNV.Columns["maNhanVien"].Width = 140;
            }
            if (dgvDanhSachNV.Columns["tenNhanVien"] != null)
            {
                dgvDanhSachNV.Columns["tenNhanVien"].HeaderText = "Tên nhân viên";
                dgvDanhSachNV.Columns["tenNhanVien"].Width = 160;
            }
            if (dgvDanhSachNV.Columns["namSinh"] != null)
            {
                dgvDanhSachNV.Columns["namSinh"].HeaderText = "Năm sinh";
                dgvDanhSachNV.Columns["namSinh"].Width = 120;
            }
            if (dgvDanhSachNV.Columns["chucVu"] != null)
            {
                dgvDanhSachNV.Columns["chucVu"].HeaderText = "Chức vụ";
                dgvDanhSachNV.Columns["chucVu"].Width = 120;
            }
            if (dgvDanhSachNV.Columns["luong"] != null)
            {
                dgvDanhSachNV.Columns["luong"].HeaderText = "Lương";
                dgvDanhSachNV.Columns["luong"].Width = 80;
            }
            if (dgvDanhSachNV.Columns["maPhong"] != null)
                dgvDanhSachNV.Columns["maPhong"].Visible = false;
            if (dgvDanhSachNV.Columns["PhongBan"] != null)
                dgvDanhSachNV.Columns["PhongBan"].Visible = false;
        }

        private void dgvDanhSachNV_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            db = new QLNVDataContext();
            if (dgvDanhSachNV.SelectedRows.Count > 0)
            {
                string manv = dgvDanhSachNV.SelectedRows[0].Cells[0].Value.ToString();
                var nhanVien = db.NhanViens.FirstOrDefault(n => n.maNhanVien == manv);
                txtMaNV.Text = nhanVien.maNhanVien;
                txtHoTen.Text = nhanVien.tenNhanVien;
                txtNamSinh.Text = nhanVien.namSinh.ToString();
                txtChucVu.Text = nhanVien.chucVu;
                txtLuong.Text = nhanVien.luong.ToString();

                AnTextBoxNV(true);
                txtMaNV.Enabled = false;
                btnLuu.Enabled = false;
                btnSua.Enabled = false;
            }
        }

        private void btnThemPhong_Click(object sender, EventArgs e)
        {
            if(btnThemPhong.Text=="Thêm phòng")
            {
                AnTextBoxPhong(false);
                btnThemPhong.Text = "Lưu";
            }
            else
            {
                try
                {
                    db = new QLNVDataContext();
                    var phong = db.PhongBans.FirstOrDefault(p => p.maPhong == int.Parse(txtMaPhong.Text));
                    if (phong == null)
                    {
                        PhongBan phongMoi = new PhongBan();
                        phongMoi.maPhong = int.Parse(txtMaPhong.Text);
                        phongMoi.tenPhong = txtTenPhong.Text;
                        db.PhongBans.InsertOnSubmit(phongMoi);
                        db.SubmitChanges();
                        LoadTreeView();
                        AnTextBoxPhong(true);
                        btnThemPhong.Text = "Thêm phòng";
                        MessageBox.Show("Thêm Phòng ban mới thành công!", "Thông báo");
                    }
                    else
                        MessageBox.Show("Mã phòng ban này đã tồn tại!", "Thông báo lỗi");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Thông báo lỗi");
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (btnThem.Text == "Thêm")
            {
                btnThem.Text = "Bỏ qua";
                AnTextBoxNV(false);
                txtMaNV.Enabled = true;
                txtMaNV.Focus();

                txtMaNV.Clear();
                txtHoTen.Clear();
                txtNamSinh.Clear();
                txtChucVu.Clear();
                txtLuong.Clear();

                btnLuu.Enabled = true;
                btnSua.Enabled = true;
            }
            else
            {
                btnThem.Text = "Thêm";
                AnTextBoxNV(true);
                txtMaNV.Enabled = false;
                btnLuu.Enabled = false;
                btnSua.Enabled = false;
            }
        }
        private bool KiemTraDuLieuNhap()
        {
            if (Convert.ToDecimal(txtLuong.Text) > 0)
            {
                if (DateTime.Now.Year - int.Parse(txtNamSinh.Text) > 23 && DateTime.Now.Year - int.Parse(txtNamSinh.Text) < 60)
                    return true;
                else
                    MessageBox.Show("Tuổi phải lớn hơn 23 và nhỏ hơn 60!", "Thông báo lỗi");
            }
            else
                MessageBox.Show("Lương phải lớn hơn 0!", "Thông báo lỗi");
            return false;
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (txtMaNV.Enabled)
            {
                try
                {
                    db = new QLNVDataContext();
                    var nv = db.NhanViens.FirstOrDefault(n => n.maNhanVien == txtMaNV.Text);
                    if (nv == null)
                    {
                        if (KiemTraDuLieuNhap())
                        {
                            NhanVien nhanVien = new NhanVien();
                            nhanVien.maNhanVien = txtMaNV.Text;
                            nhanVien.tenNhanVien = txtHoTen.Text;
                            nhanVien.namSinh = int.Parse(txtNamSinh.Text);
                            nhanVien.chucVu = txtChucVu.Text;
                            nhanVien.luong = Convert.ToDecimal(txtLuong.Text);
                            nhanVien.maPhong = (int)treDanhSachPhong.SelectedNode.Tag;

                            db.NhanViens.InsertOnSubmit(nhanVien);
                            db.SubmitChanges();
                            LoadGridView((int)treDanhSachPhong.SelectedNode.Tag);
                            btnThem.Text = "Thêm";
                            AnTextBoxNV(true);
                            txtMaNV.Enabled = false;
                            btnLuu.Enabled = false;
                            btnSua.Enabled = false;
                            MessageBox.Show("Thêm nhân viên mới thành công!", "Thông báo");
                        }
                    }
                    else
                        MessageBox.Show("Mã nhân viên này đã tồn tại!", "Thông báo lỗi");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Thông báo lỗi");
                }
            }
            else
            {
                try
                {
                    db = new QLNVDataContext();
                    var nv = db.NhanViens.FirstOrDefault(n => n.maNhanVien == txtMaNV.Text);
                    if (nv != null)
                    {
                        if (KiemTraDuLieuNhap())
                        {
                            nv.maNhanVien = txtMaNV.Text;
                            nv.tenNhanVien = txtHoTen.Text;
                            nv.namSinh = int.Parse(txtNamSinh.Text);
                            nv.chucVu = txtChucVu.Text;
                            nv.luong = Convert.ToDecimal(txtLuong.Text);
                            nv.maPhong = (int)treDanhSachPhong.SelectedNode.Tag;

                            db.SubmitChanges();
                            LoadGridView((int)treDanhSachPhong.SelectedNode.Tag);
                            btnSua.Text = "Sửa";
                            AnTextBoxNV(true);
                            btnThem.Text = "Thêm";
                            txtMaNV.Enabled = false;
                            btnLuu.Enabled = false;
                            btnSua.Enabled = false;
                            MessageBox.Show("Sửa nhân viên thành công!", "Thông báo");
                        }
                    }
                    else
                        MessageBox.Show("Chưa có mã nhân viên này!", "Thông báo lỗi");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Thông báo lỗi");
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (btnSua.Text == "Sửa")
            {
                btnSua.Text = "Bỏ qua";
                AnTextBoxNV(false);
                txtMaNV.Enabled = false;
                txtHoTen.Focus();
                btnLuu.Enabled = true;
                btnSua.Enabled = true;
            }
            else
            {
                btnSua.Text = "Sửa";
                AnTextBoxNV(true);
                txtMaNV.Enabled = false;
                btnThem.Text = "Thêm";
                btnLuu.Enabled = false;
                btnSua.Enabled = false;
            }
        }

        private void btnSapXep_Click(object sender, EventArgs e)
        {
            db = new QLNVDataContext();
            int maphong = (int)treDanhSachPhong.SelectedNode.Tag;
            List<NhanVien> lstNhanVienSX = db.NhanViens.Where(n => n.maPhong == maphong).OrderByDescending(n => n.maNhanVien).ToList();
            dgvDanhSachNV.DataSource = lstNhanVienSX;
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
