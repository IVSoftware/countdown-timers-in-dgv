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
                if(e.ColumnIndex.Equals(dataGridView.Columns["State"].Index) && (e.RowIndex != -1))
                {
                    switch (Records[e.RowIndex].State)
                    {
                        case State.WAITING: e.CellStyle.BackColor = Color.LightBlue; break;
                        case State.ACTIVE: e.CellStyle.BackColor = Color.LightYellow; break;
                        case State.EXPIRED: e.CellStyle.BackColor = Color.LightGray; break;
                        case State.FREE: e.CellStyle.BackColor = Color.LightGreen; break;
                    }
                }
            };
            Records.Clear();
            #endregion F O R M A T    C O L U M N S

            #region A D D    T E S T    R E C O R D S
            var now = DateTime.Now;
            // For testing, round to whole minutes;
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            
            Records.Add(new Record 
            { 
                IdCode = "fix0001", 
                InputTime = now - TimeSpan.FromHours(1),
                OutputTime = now - TimeSpan.FromHours(1) + TimeSpan.FromMinutes(10),
            });
            Records.Add(new Record 
            { 
                IdCode = "fix0002", 
                InputTime = now,
                OutputTime = now + TimeSpan.FromMinutes(10),
            });
            Records.Add(new Record
            {
                IdCode = "fix0003",
            });
            Records.Add(new Record
            {
                IdCode = "fix0004",
                InputTime =  now + TimeSpan.FromMinutes(5),
                OutputTime = now + TimeSpan.FromMinutes(5) + TimeSpan.FromMinutes(10),
            });
            #endregion A D D    T E S T    R E C O R D S

            // Because we're using System.Windows.Forms.Timer the
            // ticks are issued on the UI thread. Invoke not required.
            _seconds.Tick += onSecondsTick;
            _seconds.Start();
        }
        BindingList<Record> Records { get; } =
            new BindingList<Record>();
        private void onSecondsTick(object? sender, EventArgs e)
        {
            Text = $"Main Form - {DateTime.Now.ToLongTimeString()}";
            dataGridView.Refresh();
        }
        System.Windows.Forms.Timer _seconds = 
            new System.Windows.Forms.Timer { Interval = 1000 };
    }
    enum State
    {
        WAITING,
        ACTIVE,
        EXPIRED,
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
                            State = State.EXPIRED;
                            return "0";
                        }
                    }
                }
            }
        }
        public State State { get; private set; }
    }
}