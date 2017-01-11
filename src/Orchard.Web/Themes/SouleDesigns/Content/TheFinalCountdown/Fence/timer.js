var mesice = new Array(12);
mesice[0] = '';
mesice[1] = '';
mesice[2] = '';
mesice[3] = '';
mesice[4] = '';
mesice[5] = '';
mesice[6] = '';
mesice[7] = '';
mesice[8] = '­';
mesice[9] = '';
mesice[10] = '';
mesice[11] = '';

mesice[12] = 'January';
mesice[13] = 'February';
mesice[14] = 'March';
mesice[15] = 'April';
mesice[16] = 'May';
mesice[17] = 'June';
mesice[18] = 'July';
mesice[19] = 'August';
mesice[20] = 'September';
mesice[21] = 'October';
mesice[22] = 'November';
mesice[23] = 'December';

function fillCurrentDate(destid,lang)
{
	datum = new Date();
	mesic = datum.getMonth();
	den = datum.getDate();
	rok = datum.getFullYear();
	
	obj = document.getElementById(destid);
	obj.value = den+' '+mesice[mesic+(1-lang)*12]+' '+rok;
}

function fillCurrentTime()
{
	datum = new Date();
	hodiny = datum.getHours();
	minuty = datum.getMinutes();
	vteriny = datum.getSeconds();
	
	obj = document.getElementById('idhour');
	obj.selectedIndex = hodiny;
	obj = document.getElementById('idmin');
	obj.selectedIndex = minuty;
	obj = document.getElementById('idsec');
	obj.selectedIndex = 0;
}


function showuploader(value,str)
{
	obj = document.getElementById('backimagefileuploader');
	if (str != '')
		document.cdsettings.backimagefile.value = str;
	if (value == 1)
	{
		obj.style.visibility = 'visible';
	}
	else
	{
		obj.style.visibility = 'hidden';
	}
}


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

       oDiff.sech = Math.floor(nTotalDiff/100);
	   
	   if (oDiff.days < 0)
	   {
			oDiff.days = 0;
			oDiff.hours = 0;
			oDiff.minutes = 0;
			oDiff.seconds = 0;
			oDiff.sech = 0;
	   }

 
       return oDiff;
 
}

var dt_year=0;
var dt_month=0;
var dt_day=0;
var dt_hours=0;
var dt_minutes=0;
var dt_seconds=0;

function setTimerDateTime(y,mon,d,h,min,s)
{
	dt_year = y;
	dt_month = mon - 1;
	dt_day = d;
	dt_hours = h;
	dt_minutes = min;
	dt_seconds = s;
}

var timerLang = 0;

function setLang(lang)
{
	timerLang = lang;
}

function runtime()
{
	eDate = new Date();
	lDate = new Date(dt_year, dt_month, dt_day, dt_hours, dt_minutes, dt_seconds);
	diff = get_time_difference(eDate,lDate);

	obj1 = document.getElementById('days');
	obj2 = document.getElementById('hours');
	obj3 = document.getElementById('minutes');
	obj4 = document.getElementById('seconds');
	obj5 = document.getElementById('sech');
	
	if (obj1 == null)
	{
		setTimeout('runtime();', 100);
		return;
	}
	
	if (diff.days < 10) diff.days = '0'+diff.days;
	obj1.innerHTML = diff.days;

	if (diff.hours < 10) diff.hours = '0'+diff.hours;
	obj2.innerHTML = diff.hours;

	if (diff.minutes < 10) diff.minutes = '0'+diff.minutes;
	obj3.innerHTML = diff.minutes;

	if (diff.seconds < 10) diff.seconds = '0'+diff.seconds;
	obj4.innerHTML = diff.seconds;

	obj5.innerHTML = diff.sech;

	
	obj1 = document.getElementById('tdays');
	obj2 = document.getElementById('thours');
	obj3 = document.getElementById('tminutes');
	obj4 = document.getElementById('tseconds');
	obj5 = document.getElementById('tsech');
	
	obj1.innerHTML = 'Days';
	obj2.innerHTML = 'Hours';
	obj3.innerHTML = 'Minutes';
	obj4.innerHTML = 'Seconds';
	obj5.innerHTML = ' ';


	setTimeout('runtime();', 100);
}

function fillSettingsForm()
{
	d = new Date();
	var cntset = document.forms[0];
	cntset.day.value = d.getDate();
	cntset.month.value = d.getMonth()+1;
	cntset.year.value = d.getFullYear()+1;
	cntset.hours.value = d.getHours();
	cntset.minutes.value = d.getMinutes();
	cntset.seconds.value = d.getSeconds();
}

function textCounter(field,maxlimit)
{

	if (field.value.length > maxlimit)
		field.value = field.value.substring(0, maxlimit);
	else
		document.getElementById('remLen').innerHTML = (maxlimit - field.value.length);
} 
