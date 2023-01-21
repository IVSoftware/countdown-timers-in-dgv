using System.ComponentModel;

namespace countdown_timers_in_dgv
{
    public partial class MainForm : Form
    {
        public MainForm() => InitializeComponent();
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            dataGridView.DataSource = Records;
            #region F O R M A T    C O L U M N S
            Records.Add(new Record()); // <- Auto-generate columns
            dataGridView.Columns[nameof(Record.IdCode)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DataGridViewColumn col;
            col = dataGridView.Columns[nameof(Record.InputTime)];
            col.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col = dataGridView.Columns[nameof(Record.OutputTime)];
            col.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView.CellPainting += (sender, e) =>
            {
                if((e.ColumnIndex.Equals(dataGridView.Columns["State"]) && (e.RowIndex != -1)))
                {

                }
            };
            Records.Clear();
            #endregion F O R M A T    C O L U M N S

            // Add a few records
            var now = DateTime.Now;
            Records.Add(new Record 
            { 
                IdCode = "fix0001", 
                InputTime = now - TimeSpan.FromHours(1),
                OutputTime = (now - TimeSpan.FromHours(1)) + TimeSpan.FromMinutes(10),
            });
            Records.Add(new Record 
            { 
                IdCode = "fix0002", 
                InputTime = now,
                OutputTime = now + TimeSpan.FromMinutes(10),
            });
            //Records.Add(new Record 
            //{ 
            //    IdCode = "fix0001", 
            //    InputTime = now - TimeSpan.FromDays(1),
            //    OutputTime = (now - TimeSpan.FromDays(1)) + TimeSpan.FromMinutes(10),
            //});

            // Because we're using System.Windows.Forms.Timer the
            // ticks are issued on the UI thread. Invoke not required.
            _seconds.Tick += (sender, e) => dataGridView.Refresh();
            _seconds.Start();
        }
        BindingList<Record> Records { get; } =
            new BindingList<Record>();
        System.Windows.Forms.Timer _seconds = 
            new System.Windows.Forms.Timer { Interval = 1000 };
    }
    enum State
    {
        WAITING,
        ACTIVE,
        DONE,
        FREE,
    }
    class Record
    {
        [DisplayName("ID code")]
        public string IdCode { get; set; } = string.Empty;

        [DisplayName("Input time")]
        public DateTime? InputTime { get; set; }

        [DisplayName("Output time")]
        public DateTime? OutputTime { get; set; }

        // Record calculates itself when the DataGridView refreshes.
        public string Remaining
        {
            get
            {
                if((InputTime == null) || (OutputTime == null))
                {
                    State = State.FREE;
                    return string.Empty;
                }
                else
                {
                    var now = DateTime.Now;
                    // Are we inside the time window?
                    if((InputTime <= now) && (now <= OutputTime))
                    {
                        State = State.ACTIVE;
                        return (OutputTime - now)?.ToString(@"hh\:mm\:ss")!;
                    }
                    else
                    {
                        if(InputTime > now)
                        {
                            State = State.WAITING;
                            return (OutputTime - InputTime)?.ToString(@"hh\:mm")!;
                        }
                        else
                        {
                            State = State.DONE;
                            return "0";
                        }
                    }
                }
            }
        }
        public State State { get; private set; }
    }
}