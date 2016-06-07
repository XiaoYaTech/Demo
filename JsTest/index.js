window.onload=function(){
	(function(){
		var data=[1,2,3];
		var add = function(a,b)
		{
			return a+b;
		}

		var sum = data.reduce(add,0);
		console.log(sum);
		
		var j= jTool();
		j.fullPage(['page1','page2','page3','page4','page5','page6','page7']);
	})();
};