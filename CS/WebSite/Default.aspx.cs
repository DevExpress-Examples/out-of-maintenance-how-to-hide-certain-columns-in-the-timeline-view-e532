using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DevExpress.XtraScheduler;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ASPxScheduler1.ActiveViewType = SchedulerViewType.Timeline;

        TimeScaleCollection scales = ASPxScheduler1.TimelineView.Scales;

        scales.BeginUpdate();
        try {
            scales.Clear();
            scales.Add(new My20MinutesScale());
            scales.Add(new My15MinutesScale());
            scales.Add(new MyHourScale());
            scales.Add(new CustomTimeScaleDay());
        }
        finally {
            scales.EndUpdate();
        }
    }
}
