var MyModule = (function(){
	
	//工厂模式
	function createPerson(name,age){
		var o = {};
		o.name=name;
		o.age=age;
		o.sayName=function(){
			console.log(this.name);
		};
		return o;
	};
	var f = createPerson("王君实","30");
	f.sayName();
	
	
	//构造函数模式
	function Person(name,age){
		this.name=name;
		this.age=age;
		this.sayName=function(){
			console.log(this.name);
		};
	};
	var p = new Person('王君实',30)
	p.sayName();
	
	
	//原型模式
	function Person2(){
		
	};
	Person2.prototype.name="王君实";
	Person2.prototype.age=30;
	Person2.prototype.sayName=function(){ console.log(this.name);};
	
	var p = new Person2();
	p.sayName();
	
	//组合构造函数模式和原型模式
	function Person3(name age){
		this.name=name;
		this.age=age;		
	}
	Person3.prototype={
		constructor:Person3,
		sayName:function(){
			console.log(this.name);
		}
	};
	var p = new Person('王君实',30)
	p.sayName();
	
	//寄生构造函数模式，类似工厂模式，只是用new 创建了对像。
	function SpecialArray()
	{
		var arr = new Array();
		arr.push.apply(arr,arguments);
		
		arr.toPipString=function(){
			return this.join('|');
		};
		
		return arr;
	}
	var colors = new SpecialArray('red','green','white');
	colors.toPipString();
	
	
	//稳妥构造函数模式
	
})();