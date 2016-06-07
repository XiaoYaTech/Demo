(function(global){
	var version="0.1";
	var document = global.document;
	var window = global;
	
	var JTool=_j =function(){
		return new JTool.fn.init();
	};
	
	_j.fn=JTool.prototype={
		version:version,
		init:function(){
			return this;
		}
	};
	_j.extend = _j.fn.extend=function(obj){
		for(var prop in obj)
		{
			this[prop]=obj[prop];
		}
		return this;
	};
	
	Function.prototype.method = function (name,fn){
		this.prototype[name]=fn;
		return this;
	};
	
	Array.method("reduce",function(f,val){
		for(var i =0;i<this.length;i++)
		{
			val = f(this[i],val);
		}
		return val;
	});
	
	_j.fn.extend({
		fullPage:function(pages){//传入所有全屏显示的ids
			if(!pages || pages.length<=1)
			{
				return ;
			}
			var timer =null;
			var tempArr=[];
			function mouseWheel(e){
				if(timer)
				{
					return;
				}
				e = e||window.event;
				timer= setTimeout(function(){
					if(e.wheelDelta){//判断浏览器IE，谷歌滑轮事件    
						if (e.wheelDelta > 0) { //当滑轮向上滚动时
							scrollUp();
						}
						if (e.wheelDelta < 0) { //当滑轮向下滚动时
							scrollDown();
						}
					}else if(e.detail){
						if (e.detail> 0) { //当滑轮向上滚动时
							scrollUp();
						}
						if (e.detail< 0) { //当滑轮向下滚动时
							scrollDown();
						}
					}
					timer=null;
				},800);
			}
			function changeStyle(elem,dir){
					if(dir==='up')
					{
						elem.style.transform='';
					}
					else if(dir==='down'){
						
						elem.style.transform='translateY(-100%)';
					}
				}
				function Video(v)
				{
					var v = document.getElementById(v);
					this.play=function(){
						if(!!v)
						{
							v.play();
						}
					};
					this.pause=function(){
						if(!!v)
						{
							v.pause();
						}
					};
				}
				function scrollDown(){
					var p =null;
					if(pages.length>1 && (p = pages.pop()))
					{
						tempArr.push(p);
						changeStyle(document.getElementById(p),'down');
						//当前视频停止，下一个视频播放
						var v = new Video(p+"_video")
						v.pause();
						var v2 = new Video(pages[pages.length-1]+"_video")
						v2.play();
					}
				}
				function scrollUp(){
					var p = null;
					if(tempArr.length!=0 && (p = tempArr.pop()))
					{
						changeStyle(document.getElementById(p),'up');
						
						//当前视频停止，下一个视频播放
						var v = new Video(p+"_video")
						v.play();
						
						var v2 = new Video(pages[pages.length-1]+"_video")
						v2.pause();
						pages.push(p);
					}
				}
			if(document.addEventListener)
			{
				document.addEventListener('DOMMouseScroll',mouseWheel,false);
			}
			global.onmousewheel = document.onmousewheel=mouseWheel;
			
		},
		setCookie:function(c_name, value, expireHours){
			var str = c_name + "=" + escape(value); 
                if (expireHours > 0) {//为0时不设定过期时间，浏览器关闭时cookie自动消失 
                    var date = new Date(); 
                    var ms = expireHours * 3600 * 1000; 
                    date.setTime(date.getTime() + ms); 
                    str += "; expires=" + date.toGMTString(); 
                } 
                document.cookie = str; 
		},
		getCookie:function(name){//获取指定名称的cookie的值,如果name的值为空，反回所有的cookie
			if(!name)
			{
				return document.cookie;
			}
			var arr,reg=new RegExp("(^| )"+name+"=([^;]*)(;|$)");
			if(arr=document.cookie.match(reg))
				return (unescape(arr[2]));
			else
				return null;
		},
		delCookie:function(name){//为了删除指定名称的cookie，可以将其过期时间设定为一个过去的时间 
			var date = new Date(); 
			date.setTime(date.getTime() - 1); 
			var cval=this.getCookie(name);
			if(cval!=null)
				document.cookie= name + "="+cval+";expires="+exp.toGMTString();
		}
	});
	
	_j.fn.init.prototype=_j.fn;
	global.jTool=global.j$ = _j;
})(window);