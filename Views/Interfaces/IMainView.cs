using HarborMaster.Models;
﻿using HarborMaster.Presenters;
﻿using System.Collections.Generic;
﻿
﻿namespace HarborMaster.Views.Interfaces
﻿{
﻿    public interface IMainView
﻿    {
﻿        void SetScheduleDataSource(List<DashboardScheduleViewModel> schedule);
﻿        void SetBerthDataSource(List<Berth> berths);
﻿
﻿        int SelectedShipId { get; }
﻿        System.DateTime SelectedETA { get; }
﻿        System.DateTime SelectedETD { get; }
﻿
﻿        void ShowMessage(string message);
﻿        void SetCurrentUser(string fullName, bool canOverride);
﻿    }
﻿}
﻿