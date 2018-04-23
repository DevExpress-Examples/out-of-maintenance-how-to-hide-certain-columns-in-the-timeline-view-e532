# How to hide certain columns in the Timeline view


<p>Problem:</p><p>I'd like to hide columns for certain (non-working) hours in the TimelineView. How can I accomplish this?</p><p>Solution:</p><p>You can implement a custom time scale. It enables you to hide unneeded time periods.<br />
A custom time scale class is inherited from the <a href="http://documentation.devexpress.com/#WindowsForms/clsDevExpressXtraSchedulerTimeScaletopic">TimeScale</a> class, overriding methods for the DateTime data handling and formatting. <br />
You should override the following properties and methods:<br />
SortingWeight - used to compare time scales;<br />
Floor - specifies column boundaries;<br />
HasNextDate  - checks whether the move to the next date is allowed;<br />
GetNextDate - navigates to the next date;<br />
DefaultDisplayFormat - specifies the format for the column header caption;<br />
DefaultMenuCaption - specifies the caption for the context menu item which enables this time scale;<br />
FormatCaption - returns a string to be displayed as a column header caption.</p><p>To ensure a proper alignment of an upper (Day) scale, create a descendant with the Floor method overridden. Add this descendant to the scales collection in place of the default TimeScaleDay scale.</p>

<br/>


