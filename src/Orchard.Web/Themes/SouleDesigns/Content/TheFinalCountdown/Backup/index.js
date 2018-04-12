var dt_year=0;
var dt_month=0;
var dt_day=0;
var dt_hours=0;
var dt_minutes=0;
var dt_seconds=0;


function get_time_difference(earlierDate,laterDate)
{
       var nTotalDiff = laterDate.getTime() - earlierDate.getTime();
       var oDiff = new Object();
 
       oDiff.days = Math.floor(nTotalDiff/1000/60/60/24);
       nTotalDiff -= oDiff.days*1000*60*60*24;
 
       oDiff.hours = Math.floor(nTotalDiff/1000/60/60);
       nTotalDiff -= oDiff.hours*1000*60*60;
 
       oDiff.minutes = Math.floor(nTotalDiff/1000/60);
       nTotalDiff -= oDiff.minutes*1000*60;
 
       oDiff.seconds = Math.floor(nTotalDiff/1000);
       nTotalDiff -= oDiff.seconds*1000;

	   if (oDiff.days < 0)
	   {
			oDiff.days = 0;
			oDiff.hours = 0;
			oDiff.minutes = 0;
			oDiff.seconds = 0;
	   }

 
       return oDiff;
 
}

function setTimerDateTime(y,mon,d,h,min,s)
{
	dt_year = y;
	dt_month = mon - 1;
	dt_day = d;
	dt_hours = h;
	dt_minutes = min;
	dt_seconds = s;
}

function runtime()
{
	eDate = new Date();
	lDate = new Date(dt_year, dt_month, dt_day, dt_hours, dt_minutes, dt_seconds);
	diff = get_time_difference(eDate,lDate);

	// Pad with zeros - assure string
	if (diff.days < 100 && diff.days >= 10) 
		diff.days = '0' + diff.days;
	else if (diff.days < 10) 
		diff.days = '00'+diff.days;
	else
		diff.days = '' + diff.days + '';

	if (diff.hours < 10) 	
		diff.hours = '0'+diff.hours;
	else
		diff.hours = '' + diff.hours + '';

	if (diff.minutes < 10) 
		diff.minutes = '0'+diff.minutes;
	else
		diff.minutes = '' + diff.minutes + '';

	if (diff.seconds < 10) 
		diff.seconds = '0'+diff.seconds;
	else
		diff.seconds = '' + diff.seconds + '';


	// Loop across all counters and set appropriate values
	for(var index = 0; index < counters.length; index++) {
		var counter = counters[index];

		// Get counter's index
		var charIndex = counter.divId.substring(counter.divId.indexOf("-") + 1);

		if(counter.divId.indexOf("day") != -1) {
			counter.setValue(parseInt(diff.days.substr(charIndex, 1)));
		} else if(counter.divId.indexOf("hour") != -1) {
			counter.setValue(parseInt(diff.hours.substr(charIndex, 1)));
		} else if(counter.divId.indexOf("minute") != -1) {
			counter.setValue(parseInt(diff.minutes.substr(charIndex, 1)));
		} else if(counter.divId.indexOf("second") != -1) {
			counter.setValue(parseInt(diff.seconds.substr(charIndex, 1)));
		}
	}	

	setTimeout('runtime();', 1000);
}
