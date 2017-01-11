$(document).ready(function() {	
		// Setup timer
		setTimerDateTime(2017, 03, 01, 08, 00, 00, 0);
		setTimeout('runtime();',50)
		
		// Switch to pause button
		var audio = $("audio")[0];
		audio.onplay = function() {
			$("#play-button span").text('pause');
			$("#play-button .fa").removeClass("fa-play").addClass("fa-pause");
		};
		audio.onpause = function() {
			$("#play-button span").text('play');
			$("#play-button .fa").removeClass("fa-pause").addClass("fa-play");
		}
		
		// Simple toggle on/off sound
		$("#play-button").click(function(){
			try
			{
				var audioplayer = $("audio")[0];		
				if (audioplayer.paused) {			
					audioplayer.play();
				}   
				else {				
					audioplayer.pause();
				}
			} catch(ex) {}
		});
	});