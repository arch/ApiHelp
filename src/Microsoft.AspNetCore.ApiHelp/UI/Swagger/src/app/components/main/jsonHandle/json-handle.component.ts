import { Component, OnInit, Input, EventEmitter, OnChanges, SimpleChanges, AfterViewChecked} from '@angular/core';
import { Observable } from 'rxjs/Observable';

@Component({
    moduleId: module.id,
    selector: 'json-handle',
    templateUrl: 'json-handle.component.html',
    styleUrls: ['json-handle.component.css']
})
export class JsonHandleComponent implements OnInit, AfterViewChecked {
    constructor() { }

    @Input() jsonStr;
    ngOnChanges(changes: SimpleChanges) {
        this.HTML = this.Process();
    }
    ngOnInit() {
        this.HTML = this.Process();
    }
    ngAfterViewChecked() {
        console.clear();
    }

    SINGLE_TAB = "  ";
    TAB = this.MultiplyString(1, this.SINGLE_TAB);
    ImgCollapsed = "./images/Collapsed.gif";
    ImgExpanded = "./images/Expanded.gif";
    HTML: string = "";

    @Input() showType: string = "code";

    // Indent
    MultiplyString(num: number, str: string) {
        var sb = [];
        for (var i = 0; i < num; i++) {
            sb.push(str);
        }
        return sb.join("");
    }
    Process() {
        var html = "";
        var HTML = "";
        //if (this.jsonStr == "")
        //    this.jsonStr = "\"\"";
        var obj = this.jsonStr;
        html = this.ProcessObject(obj, 0, false, false, false);
        if (this.showType == "code")
            HTML = `<PRE class='CodeContainer canEdit'>${html}</PRE>`;
        else if (this.showType == "text")
            HTML = `<PRE class='CodeContainer canEdit' contenteditable="true">${html}</PRE>`;
        return HTML;
    }
    getDataType(data) {
        if (typeof data === 'object') {
            if (data instanceof Array)
                return 'array';
            else if (data instanceof Date)
                return 'date';
            else if (data instanceof RegExp)
                return 'regExp';
            else
                return 'object'
        }
        else {
            return typeof data;
        }
    }
    GetRow(indent, data, isPropertyContent) {
        var tabs = "";
        for (var i = 0; i < indent && !isPropertyContent; i++) tabs += this.TAB;
        if (data != null && data.length > 0 && data.charAt(data.length - 1) != "\n")
            data = data + "\n";
        return tabs + data;
    }
    FormatLiteral(literal, quote, comma, indent, isArray, style) {
        if (typeof literal == 'string')
            literal = literal.split("<").join("&lt;").split(">").join("&gt;");
        var str = "<span class='" + style + "'>" + quote + literal + quote + comma + "</span>";
        if (isArray) str = this.GetRow(indent, str, null);
        return str;
    }
    FormatFunction(indent, obj) {
        var tabs = "";
        for (var i = 0; i < indent; i++) tabs += this.TAB;
        var funcStrArray = obj.toString().split("\n");
        var str = "";
        for (var i = 0; i < funcStrArray.length; i++) {
            str += ((i == 0) ? "" : tabs) + funcStrArray[i] + "\n";
        }
        return str;
    }
    ProcessObject(obj, indent, addComma, isArray, isPropertyContent) {
        var html = "";
        var comma = (addComma) ? "<span class='Comma'>,</span> " : "";
        var type = this.getDataType(obj);
        var clpsHtml = "";
        if (type == 'array') {
            if (obj.length == 0) {
                html += this.GetRow(indent, `<span class='ArrayBrace'>[ ]</span>${comma}`, isPropertyContent);
            } else {
                clpsHtml = `<span class="img_click"><img src="${this.ImgExpanded}"  /></span><span class='collapsible'>`;
                html += this.GetRow(indent, `<span class='ArrayBrace'>[</span>${clpsHtml}`, isPropertyContent);
                for (var i = 0; i < obj.length; i++) {
                    html += this.ProcessObject(obj[i], indent + 1, i < (obj.length - 1), true, false);
                }
                clpsHtml = "</span>";
                html += this.GetRow(indent, clpsHtml + "<span class='ArrayBrace'>]</span>" + comma, null);
            }
        } else if (type == 'object') {
            if (obj == null) {
                html += this.FormatLiteral("null", "", comma, indent, isArray, "Null");
            } else if (type == 'date') {
                html += this.FormatLiteral(`new Date(${obj.getTime()}) /*${obj.toLocaleString()}*/`, "", comma, indent, isArray, "Date");
            } else if (type == 'regExp') {
                html += this.FormatLiteral(`new RegExp(${obj})`, "", comma, indent, isArray, "RegExp");
            } else {
                var numProps = 0;
                for (var prop in obj) numProps++;
                if (numProps == 0) {
                    html += this.GetRow(indent, "<span class='ObjectBrace'>{ }</span>" + comma, isPropertyContent);
                } else {
                    clpsHtml = `<span class="img_click"><img src="${this.ImgExpanded}" /></span><span class='collapsible'>`;
                    html += this.GetRow(indent, `<span class='ObjectBrace'>{</span>${clpsHtml}`, isPropertyContent);
                    var j = 0;
                    for (var prop in obj) {
                        var quote = "\"";
                        var result = this.ProcessObject(obj[prop], indent + 1, ++j < numProps, false, true);
                        html += this.GetRow(indent + 1, `<span class='PropertyName'>${quote}${prop}${quote}</span>: ${result}`, null);
                    }
                    clpsHtml = "</span>";
                    html += this.GetRow(indent, `${clpsHtml}<span class='ObjectBrace'>}</span>${comma}`, null);
                }
            }
        } else if (type == 'number') {
            html += this.FormatLiteral(obj, "", comma, indent, isArray, "Number");
        } else if (type == 'boolean') {
            html += this.FormatLiteral(obj, "", comma, indent, isArray, "Boolean");
        } else if (type == 'function') {
            if (type == 'regExp') {
                html += this.FormatLiteral(`new RegExp(${obj})`, "", comma, indent, isArray, "RegExp");
            } else {
                obj = this.FormatFunction(indent, obj);
                html += this.FormatLiteral(obj, "", comma, indent, isArray, "Function");
            }
        } else if (type == 'undefined') {
            html += this.FormatLiteral("undefined", "", comma, indent, isArray, "Null");
        } else {
            html += this.FormatLiteral(obj.toString().split("\\").join("\\\\").split('"').join('\\"'), "\"", comma, indent, isArray, "String");
        }
        return html;
    }
    test() {
        // this.onGetValue.emit("test");
    }
}