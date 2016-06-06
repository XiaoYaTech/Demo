/**
 * i18n: JavaScript function
 * Project: 
 * Release: Working copy
 * Locale: en, 
 * Exported at: Thu, 05 Feb 2015 14:15:42 GMT 
 */
// First, checks if it isn't implemented yet.
if (!String.prototype.format) {
  String.prototype.format = function() {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function(match, number) { 
      return typeof args[number] != 'undefined'
        ? args[number]
        : match
      ;
    });
  };
}
var _ = function( pairs ){
    
    // calc numeric index of a plural form
    function pluralIndex( n ){
        return Number( (n != 1) );
    }

    function lookup(msgid1, msgid2, n) {
        var value = pairs[msgid1];
        // singular if no multiplier
        if (null == n) {
            n = 1;
        }
        // plurals stored as objects, e.g. { 0: '' }
        if (value instanceof Object) {
            value = value[pluralIndex(n) || 0];
        }
        return value || (1 === n ? msgid1 : msgid2) || msgid1 || '';
    }

    function format(str, args) {
        return str.replace(/%(\d+)/g, function (match, number) {
            return typeof args[number] != 'undefined' ? args[number] : match;
        });
    }

    function parse(str, p1, p2, p3, offset, s) {

        // Extract msgid from 1nd capture group.
        var splitMsgId = p1.split('\!\!\!');
        var MsgId = splitMsgId[0];
        var MsgIdPlural = splitMsgId.length > 1 ? splitMsgId[1] : '';
        // Extract format items from 2rd capture group.
        var formatItems = p2.split('\|\|\|');
        // Extract comment from 3th capture group.
        var Comment = p3;
        return format(lookup(MsgId, MsgIdPlural, Number(formatItems[0])), formatItems);
    }

    // expose public t() function
    return function (nuggetStr) {
        return nuggetStr.replace(/\[\[\[(.+?)(?:\|\|\|(.+?))(?:\/\/\/(.+?))?\]\]\]/, parse);
    }
}( { "multi language test":"it's just a joke" } )