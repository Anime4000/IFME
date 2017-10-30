namespace ifme
{
    public partial class frmProgressBar
    {
        public string Status
        {
            get { return lblStatus.Text; }
            set { lblStatus.Text = value; }
        }

        public int Progress
        {
            get { return pbLoading.Value; }
            set { try { pbLoading.Value = value; } catch { } }
        }
    }
}
