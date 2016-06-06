declare class AjaxQueue {
    constructor(name:string);
    Request(requestKey: string, xhrOptions: {}):AjaxQueue;
    Run(): AjaxQueue;
    Abort(requestKey: string): AjaxQueue;
}