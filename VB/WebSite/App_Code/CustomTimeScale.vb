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

'     Implement a base class for the scales of less than a day.
'     * The visible time starts at 6AM on working days, 8AM on Saturdays, 9AM on Sundays.
'     * End of the visible interval corresponds to 3PM on working days, 12PM on weekends.  
'     
	Public MustInherit Class LessThanADayTimeScale
		Inherits TimeScale
		' Holds the scale increment, in minutes.
		Private scaleValue As Integer
		Private scaleValueTime As TimeSpan

		Public Sub New(ByVal ScaleValue As Integer)
			Me.scaleValue = ScaleValue
			scaleValueTime = TimeSpan.FromMinutes(ScaleValue)
		End Sub

		' Gets the start of the first time interval.
		Protected Overridable Function FirstIntervalStart(ByVal [date] As DateTime) As Integer
			Select Case [date].DayOfWeek
				Case DayOfWeek.Saturday
					Return 8 * 60
				Case DayOfWeek.Sunday
					Return 9 * 60
				Case Else
					Return 6 * 60
			End Select
		End Function

		' Gets the start of the last time interval.
		Protected Overridable Function LastIntervalStart(ByVal [date] As DateTime) As Integer
			Select Case [date].DayOfWeek
				Case DayOfWeek.Saturday
					Return 12 * 60 - scaleValue
				Case DayOfWeek.Sunday
					Return 12 * 60 - scaleValue
				Case Else
					Return 15 * 60 - scaleValue
			End Select
		End Function

		' Gets the value used to order the scales.
		Protected Overrides ReadOnly Property SortingWeight() As TimeSpan
			Get
				Return TimeSpan.FromMinutes(scaleValue + 1)
			End Get
		End Property

		Public Overrides Function Floor(ByVal [date] As DateTime) As DateTime

			' Performs edge calculations.
			If [date] = DateTime.MinValue OrElse [date] = DateTime.MaxValue Then
				Return RoundToScaleInterval([date])
			End If

			' Rounds down to the last interval in the previous date.
			If [date].TimeOfDay.TotalMinutes < FirstIntervalStart([date]) Then
				Return RoundToVisibleIntervalEdge([date].AddDays(-1), LastIntervalStart([date]))
			End If

			' Rounds down to the last interval in the current date.
			If [date].TimeOfDay.TotalMinutes > LastIntervalStart([date]) Then
				Return RoundToVisibleIntervalEdge([date], LastIntervalStart([date]))
			End If

			' Rounds down to the scale node.
			Return RoundToScaleInterval([date])
		End Function

		Protected Function RoundToVisibleIntervalEdge(ByVal dateTime As DateTime, ByVal minutes As Integer) As DateTime
			Return dateTime.Date.AddMinutes(minutes)
		End Function
		Protected Function RoundToScaleInterval(ByVal [date] As DateTime) As DateTime
			Return DevExpress.XtraScheduler.Native.DateTimeHelper.Floor([date], TimeSpan.FromMinutes(scaleValue))
		End Function
		' Checks for edge conditions.
		Protected Overrides Function HasNextDate(ByVal [date] As DateTime) As Boolean
			Return [date] <= (DateTime.MaxValue - scaleValueTime)
		End Function

		Public Overrides Function GetNextDate(ByVal [date] As DateTime) As DateTime
			If HasNextDate([date]) Then
				If ([date].TimeOfDay.TotalMinutes > LastIntervalStart([date]) - scaleValue) Then
					Return RoundToVisibleIntervalEdge([date].AddDays(1), FirstIntervalStart([date].AddDays(1)))
				Else
					Return [date].AddMinutes(scaleValue)
				End If
			Else
				Return [date]
			End If
		End Function
	End Class

#Region " Custom Scales Implementation"
	Public Class My20MinutesScale
		Inherits LessThanADayTimeScale
		Private Const myScaleValue As Integer = 20
		Public Sub New()
			MyBase.New(myScaleValue)
		End Sub

		Protected Overrides ReadOnly Property DefaultDisplayFormat() As String
			Get
				Return "HH:mm"
			End Get
		End Property

		Protected Overrides ReadOnly Property DefaultMenuCaption() As String
			Get
				Return "My20Minutes"
			End Get
		End Property

	End Class
	Public Class My15MinutesScale
		Inherits LessThanADayTimeScale
		Private Const myScaleValue As Integer = 15
		Public Sub New()
			MyBase.New(myScaleValue)
		End Sub

		Protected Overrides ReadOnly Property DefaultDisplayFormat() As String
			Get
				Return "HH:mm"
			End Get
		End Property

		Protected Overrides ReadOnly Property DefaultMenuCaption() As String
			Get
				Return "My15Minutes"
			End Get
		End Property

	End Class

	Public Class MyHourScale
		Inherits LessThanADayTimeScale
		Private Const myScaleValue As Integer = 60
			Public Sub New()
				MyBase.New(myScaleValue)
			End Sub


		Protected Overrides ReadOnly Property DefaultDisplayFormat() As String
			Get
				Return "hh tt"
			End Get
		End Property

		Protected Overrides ReadOnly Property DefaultMenuCaption() As String
			Get
				Return "MyHour"
			End Get
		End Property

	End Class


Public Class CustomTimeScaleDay
	Inherits LessThanADayTimeScale

	Private Const myScaleValue As Integer = 1440
		Public Sub New()
			MyBase.New(myScaleValue)
		End Sub

	Public Overrides Function Floor(ByVal [date] As DateTime) As DateTime

		' Performs edge calculations.
		If [date] = DateTime.MinValue Then
			Return [date].AddMinutes(FirstIntervalStart([date]))
		End If

		Dim start As DateTime = [date].Date

		' Rounds down to the previous date.
		If [date].TimeOfDay.TotalMinutes < FirstIntervalStart(start) Then
			start = start.AddDays(-1)
		End If


		' Rounds down to the scale node.
		Return start.AddMinutes(FirstIntervalStart(start))
	End Function

	Protected Overrides ReadOnly Property DefaultDisplayFormat() As String
		Get
			Return "d ddd"
		End Get
	End Property

	Protected Overrides ReadOnly Property DefaultMenuCaption() As String
		Get
			Return "CustomDay"
		End Get
	End Property

End Class
#End Region