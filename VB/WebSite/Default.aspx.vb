Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports DevExpress.XtraScheduler

Partial Public Class _Default
	Inherits System.Web.UI.Page
	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		ASPxScheduler1.ActiveViewType = SchedulerViewType.Timeline

		Dim scales As TimeScaleCollection = ASPxScheduler1.TimelineView.Scales

		scales.BeginUpdate()
		Try
			scales.Clear()
			scales.Add(New My20MinutesScale())
			scales.Add(New My15MinutesScale())
			scales.Add(New MyHourScale())
			scales.Add(New CustomTimeScaleDay())
		Finally
			scales.EndUpdate()
		End Try
	End Sub
End Class
