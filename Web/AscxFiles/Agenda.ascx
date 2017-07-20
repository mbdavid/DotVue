<%@ Control Language="C#" %>
<script runat="server">

    public class Agenda : ViewModel
    {
	    public Data data = new Data();
	    public string SomeTitle = "some title";

	    public Agenda()
	    {
	    }

	    [Ready]
	    protected override void OnCreated()
	    {
		    data.Days = new List<Day>();
			
		    data.DateFormat = "";
		    data.TimeFormat = "";

		    List<TimeSlot> TimeSlots = new List<TimeSlot>
		    {
			    new TimeSlot() { Time = new Time(){ Start="10:00", End="11:00" }, Title = "slot #1", Live = true },
			    new TimeSlot() { Time = new Time(){ Start="11:00", End="" }, Title = "slot #2", Live = true },
			    new TimeSlot() { Time = new Time(){ Start="12:00", End="00:00" }, Title = "slot #3", Live = true }
		    };

		    data.Days.Add(new Day() { Date = "2017-07-19T00:00:00", TimeSlots = TimeSlots });

		    TimeSlots = new List<TimeSlot>
		    {
			    new TimeSlot() { Time = new Time(){ Start="10:30", End="11:30" }, Title = "slot #4", Live = true },
			    new TimeSlot() { Time = new Time(){ Start="11:30", End="12:30" }, Title = "slot #5", Live = true },
			    new TimeSlot() { Time = new Time(){ Start="12:30", End="13:30" }, Title = "slot #6", Live = true }
		    };

		    data.Days.Add(new Day() { Date = "2017-07-20T00:00:00", TimeSlots = TimeSlots });
	    }

	    public void Save(string newTitle)
	    {
		    SomeTitle = newTitle;
	    }
    }

    public struct Data
    {
	    public List<Day> Days { get; set; }
	    public string DateFormat { get; set; }
	    public string TimeFormat { get; set; }
    }

    public struct Day
    {
	    public string Date { get; set; }
	    public List<TimeSlot> TimeSlots { get; set; }
    }

    public struct TimeSlot
    {
	    public Time Time { get; set; }
	    public string Title { get; set; }
	    public bool Live { get; set; }
    }

    public struct Time
    {
	    public string Start { get; set; }
	    public string End { get; set; }
    }

</script>
<template>
    <div>
        <button @click="Save('new text')">Send to Server :: {{ SomeTitle }}</button>
        <pre>$data: {{ $data }}</pre>
        <pre>$props: {{ $props }}</pre>
    </div>
</template>