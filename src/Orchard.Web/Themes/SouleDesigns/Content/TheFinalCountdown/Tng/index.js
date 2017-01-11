$(document).ready(function() {	
		// Setup timer
		setTimerDateTime(2015, 11, 04, 09, 00, 00, 0);
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
		
		$("#weeks-8").click(function() {
			$.fancybox([
				{ 
					href: 'https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/ultrasound-2015-03-24-1.jpg',
					title: 'Week 8 - Only one image in the first ultrasound session, the baby is about the size of a pea!'
				}
			]);
		});
		
		$("#weeks-12").click(function() {
			$.fancybox([
				{
					href: 'https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/ultrasound-2015-04-23-1.jpg',
					title: 'Week 12 - 1 of 6 - Can really start to see form now!'
				},
				{
					href: 'https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/ultrasound-2015-04-23-2.jpg',
					title: 'Week 12 - 2 of 6 - baby is about 6cm long or around 2 inches!'
				},
				{
					href: 'https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/ultrasound-2015-04-23-3.jpg',
					title: 'Week 12 - 3 of 6'
				},
				{
					href: 'https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/ultrasound-2015-04-23-4.jpg',
					title: 'Week 12 - 4 of 6'
				},
				{
					href: 'https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/ultrasound-2015-04-23-5.jpg',
					title: 'Week 12 - 5 of 6'
				},
				{
					href: 'https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/ultrasound-2015-04-23-6.jpg',
					title: 'Week 12 - 6 of 6'
				}
			]);
		});		
		
		$("#weeks-20").click(function() {
			$.fancybox([
				{
					href: "https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/lks-ultrasound-2015-06-08-bodyprofile.jpg",
					title: "Week 20 - 1 of 7 - Profile shot of the baby, he is still a little under a pound"
				},
				{
					href: "https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/lks-ultrasound-2015-06-08-headprofile.jpg",
					title: "Week 20 - 2 of 7 - Baby's head in profile, can see lips, nose, the works!"
				},
				{
					href: "https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/lks-ultrasound-2015-06-08-face-1.jpg",
					title: "Week 20 - 3 of 7 - Somewhat hard to make out but this is looking right at his face"
				},
				{
					href: "https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/lks-ultrasound-2015-06-08-face-2.jpg",
					title: "Week 20 - 4 of 7 - Another face shot, the tech said there is almost no fat at this stage but you can see nostrils, lips, etc."
				},
				{
					href: "https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/lks-ultrasound-2015-06-08-leftleg.jpg",
					title: "Week 20 - 5 of 7 - Left leg, confirmed 5 toes and also confirmed 5 fingers on the left hand"
				},
				{
					href: "https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/lks-ultrasound-2015-06-08-rightleg.jpg",
					title: "Week 20 - 6 of 7 - Right leg, confirmed 5 toes and also confirmed 5 fingers on the right hand"
				},
				{
					href: "https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/lks-ultrasound-2015-06-08-bottom.jpg",
					title: "Week 20 - 7 of 7 - Baby's bottom and privates, confirmed he's a boy from his wee willy winky ;-)"
				}
			]);
		});
		
		$("#weeks-34").click(function() {
			$.fancybox([
				{
					href: "https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/ultrasound-2015-09-15-profile.jpg",
					title: "Week 34 - 1 of 3 - A baby profile shot, we went in for this ultrasound to confirm his weight was OK and he was with the normal range so all is good!"
				},
				{
					href: "https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/ultrasound-2015-09-15-hand.jpg",
					title: "Week 34 - 2 of 3 - One of his hands, he was in a strange position in the womb so we didn't get very many clear pictures on this visit!"
				},
				{
					href: "https://www.souledesigns.com/Themes/SouleDesigns/Content/TheFinalCountdown/Tng/images/ultrasound-2015-09-15-boy.jpg",
					title: "Week 34 - 3 of 3 - Looking at baby's bottom and private parts again, confirmed he's still a boy :-)"
				}
			]);
		});
	});