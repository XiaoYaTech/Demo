var MyModule = (function(){
	
	//����ģʽ
	function createPerson(name,age){
		var o = {};
		o.name=name;
		o.age=age;
		o.sayName=function(){
			console.log(this.name);
		};
		return o;
	};
	var f = createPerson("����ʵ","30");
	f.sayName();
	
	
	//���캯��ģʽ
	function Person(name,age){
		this.name=name;
		this.age=age;
		this.sayName=function(){
			console.log(this.name);
		};
	};
	var p = new Person('����ʵ',30)
	p.sayName();
	
	
	//ԭ��ģʽ
	function Person2(){
		
	};
	Person2.prototype.name="����ʵ";
	Person2.prototype.age=30;
	Person2.prototype.sayName=function(){ console.log(this.name);};
	
	var p = new Person2();
	p.sayName();
	
	//��Ϲ��캯��ģʽ��ԭ��ģʽ
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
	var p = new Person('����ʵ',30)
	p.sayName();
	
	//�������캯��ģʽ�����ƹ���ģʽ��ֻ����new �����˶���
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
	
	
	//���׹��캯��ģʽ
	
})();