// 쿠키 저장
function setCookie(name, value, exp) {
    var date = new Date();
    date.setTime(date.getTime() + exp * 24 * 60 * 60 * 1000);
    document.cookie = name + '=' + value + ';expires=' + date.toUTCString() + ';path=/';
}
function getCookie(name) {
    var value = document.cookie.match('(^|;) ?' + name + '=([^;]*)(;|$)');
    return value ? value[2] : null;
}
function deleteCookie(name) {
    var date = new Date(); // 현재시각으로 유효기간을 설정하여 바로 삭제됨
    document.cookie = name + "= " + ";expires=" + date.toUTCString() + ";path=/";
}
// 경로 지정용
function setCookieAt(name, value, exp, path) {
    var date = new Date();
    date.setTime(date.getTime() + exp * 24 * 60 * 60 * 1000);
    document.cookie = name + '=' + value + ';expires=' + date.toUTCString() + ';path=' + path;
}
function deleteCookieAt(name, path) {
    var date = new Date(); // 현재시각으로 유효기간을 설정하여 바로 삭제됨
    document.cookie = name + "= " + ";expires=" + date.toUTCString() + ";path=" + path;
}

// isEmpty
function fnIsEmpty(pValue) {
    if (pValue == "" || pValue == null || pValue == undefined) {
        return true;
    }
    else if (typeof pValue == 'object' && pValue != null) {
        if (pValue.constructor.toString().indexOf("Date") > -1) {
            return false;
        }
        else if (!Object.keys(pValue).length) {
            return true;
        }
        else {
            return false;
        }
    }
    else {
        return false;
    }
}

// Date -> yyyyMMdd 변환 함수.
function fnDateToStr(pDate) {
    if (fnIsEmpty(pDate))
        return '';

    var year = String(pDate.getFullYear()).padStart(4, "0");
    var month = String(pDate.getMonth() + 1).padStart(2, "0");
    var day = String(pDate.getDate()).padStart(2, "0");

    var yyyyMMdd = year + month + day;
    return yyyyMMdd;
}

function fnDateToStrWithSlash(pDate) {
    if (fnIsEmpty(pDate))
        return '';

    var year = String(pDate.getFullYear()).padStart(4, "0");
    var month = String(pDate.getMonth() + 1).padStart(2, "0");
    var day = String(pDate.getDate()).padStart(2, "0");

    var yyyyMMdd = year + '/' + month + '/' + day;
    return yyyyMMdd;
}

// yyyy/MM/dd hh:mm:ss -> yyyyMMddhhmmss 변환 함수
function fnStrToStr(pDate) {
    if (fnIsEmpty(pDate)) return;
    var len = pDate.length;

    var y = pDate.substr(0, 4);
    var m = pDate.substr(5, 2);
    var d = pDate.substr(8, 2);

    var sec = pDate.substr(len - 2, 2);
    var min = pDate.substr(len - 5, 2);
    var h = pDate.substr(len - 8, 2);

    var result = y + m + d + h + min + sec;
    return result;
}

// yyyyMMddhhmmss -> Date 변환함수
function fnStrToDateTime(pDate) {
    var y = pDate.substr(0, 4);
    var m = pDate.substr(4, 2);
    var d = pDate.substr(6, 2);
    var h = pDate.substr(8, 2);
    var min = pDate.substr(10, 2);
    var sec = pDate.substr(12, 2);
    return new Date(y, m - 1, d, h, min, sec);
}

// yyyyMMdd -> Date 변환함수.
function fnStrToDate(pDate) {
    var y = pDate.substr(0, 4);
    var m = pDate.substr(4, 2);
    var d = pDate.substr(6, 2);
    return new Date(y, m - 1, d);
}

// yyyy/MM/dd -> Date 변환함수.
function fnStrperToDate(pDate) {
    var y = pDate.substr(0, 4);
    var m = pDate.substr(5, 2);
    var d = pDate.substr(8, 2);
    return new Date(y, m - 1, d);
}

// DateTime -> yyyyMMdd 변환 함수.
function fnDateTimeToStr(pDate) {
    var year = String(pDate.getFullYear()).padStart(4, "0");
    var month = String(pDate.getMonth() + 1).padStart(2, "0");
    var day = String(pDate.getDate()).padStart(2, "0");

    var hour = String(pDate.getHours()).padStart(2, "0");
    var min = String(pDate.getMinutes()).padStart(2, "0");
    var sec = String(pDate.getSeconds()).padStart(2, "0");

    var yyyyMMddHHmmss = year + month + day + hour + min + sec;
    return yyyyMMddHHmmss;
}
// DateTime -> hhmm 변환 함수
function fnTimeToStr(pDate) {
    if (fnIsEmpty(pDate)) return null;
    var hour = String(pDate.getHours()).padStart(2, "0");
    var min = String(pDate.getMinutes()).padStart(2, "0");

    var HHmm = hour + min;
    return HHmm;
}
// hhmm -> DateTime (BootstrapTimeEdit) 변환 함수
function fnStrToTime(pHHMM) {
    if (fnIsEmpty(pHHMM)) return null;
    var today = new Date();

    let hour = pHHMM.substring(0, 2);
    let min = pHHMM.substring(2, 4)
    today.setHours(hour);
    today.setMinutes(min);

    return today;
}
function fnToDay() {
    var today = new Date();

    var year = today.getUTCFullYear();
    var month = today.getUTCMonth();
    var day = today.getUTCDate();

    return new Date(year, month, day);
}

// GridView의 컬럼 값을 string 형식으로 변환 후 리턴
function GridDataToString(pColumnData) {
    var lobjType = typeof (pColumnData);

    if (pColumnData == null)
        return '';

    if (lobjType == 'object') {
        //console.log(pColumnData);
        if (pColumnData.constructor.toString().indexOf("Date") > -1) {
            // 날자로 변환 [type : yyyyMMddhhmmss]            
            if (String(pColumnData.getHours()).padStart(2, "0") + String(pColumnData.getMinutes()).padStart(2, "0") + String(pColumnData.getSeconds()).padStart(2, "0") == "000000") {
                // 시/분/초가 모두 0인 경우 : 000000 -> 날짜(yyyymmdd)로 변환.                
                return fnDateToStr(pColumnData);
            }
            else {
                // 시/분/초가 존재 하는 경우 : DateTime(yyyyMMddhhmmss)로 변환
                return fnDateTimeToStr(pColumnData);
            }
        }
        else
            return pColumnData;
    }
    else {
        return pColumnData;
    }
}


// 그리드의 수정사항을 JSON으로 변환
function MakeIUDTable(pGrid) {

    var lTable = new Array();

    // Header Setting.
    var ltemp = new Array();
    ltemp[0] = 'IUD';
    for (var i = 0; i < pGrid.columns.length; i++) {
        if (fnIsEmpty(pGrid.columns[i]["fieldName"]))
            continue;

        ltemp[ltemp.length] = pGrid.columns[i]["fieldName"];
    }
    lTable[0] = ltemp;

    // Delete Row Index Setting
    var lDeletedRowIndex = pGrid.batchEditApi.GetDeletedRowIndices();
    for (var i = 0; i < lDeletedRowIndex.length; i++) {
        var ltemp = new Array();
        ltemp[0] = 'D';
        for (var j = 0; j < pGrid.columns.length; j++) {
            if (fnIsEmpty(pGrid.columns[j]["fieldName"]))
                continue;

            ltemp[ltemp.length] = GridDataToString(pGrid.batchEditApi.GetCellValue(lDeletedRowIndex[i], pGrid.columns[j]));
        }
        lTable[lTable.length] = ltemp;
    }


    console.log(lTable);

    // Update Row Index Setting
    var lUpdatedRowIndex = pGrid.batchEditApi.GetUpdatedRowIndices();
    for (var i = 0; i < lUpdatedRowIndex.length; i++) {
        var ltemp = new Array();
        ltemp[0] = 'U';
        for (var j = 0; j < pGrid.columns.length; j++) {
            if (fnIsEmpty(pGrid.columns[j]["fieldName"]))
                continue;

            ltemp[ltemp.length] = GridDataToString(pGrid.batchEditApi.GetCellValue(lUpdatedRowIndex[i], pGrid.columns[j]));
        }
        lTable[lTable.length] = ltemp;
    }

    // Insert Row Index Setting
    var lInsertedRowIndex = pGrid.batchEditApi.GetInsertedRowIndices();
    for (var i = 0; i < lInsertedRowIndex.length; i++) {
        var ltemp = new Array();
        ltemp[0] = 'I';
        for (var j = 0; j < pGrid.columns.length; j++) {
            if (fnIsEmpty(pGrid.columns[j]["fieldName"]))
                continue;

            ltemp[ltemp.length] = GridDataToString(pGrid.batchEditApi.GetCellValue(lInsertedRowIndex[i], pGrid.columns[j]));
        }
        lTable[lTable.length] = ltemp;
    }

    return JSON.stringify(lTable);
}

function fnNVL(pValue, pNullValue) {
    var lRtnValue = '';
    if (fnIsEmpty(pValue) == true) {
        lRtnValue = pNullValue;
    }
    else {
        lRtnValue = pValue;
    }

    return lRtnValue;
}

// Message Load
function fnGetMsg(pHfdMessage, pMsgID) {
    var lMsg = '';
    try {
        lMsg = pHfdMessage.Get(pMsgID);
    }
    catch (error) {
        console.log(error);
    }

    return lMsg;
}

function fnPageMethodError(pData) {
    if (pData._message == "S") {
        // 세션에러(Login Page로 이동해야됨)
        var returnUrl = location.href.replace(location.origin, '').replace(/&/gi, "%26").substring(1);
        location.href = location.origin + "/Login.aspx?ReturnUrl=" + returnUrl;
    }
    else if (pData._message == "A") {
        // 권한 에러(Home으로 이동.)
        location.href = location.origin + "/AuthorityError.aspx";
    }
}

function fnSetDeviceWidth() {
    if (window.matchMedia('(orientation: portrait)').matches) {
        // Portrait 모드일 때 실행할 스크립트
        // 폭과 높이가 같으면 Portrait 모드로 인식
        setCookie('deviceWidth', window.screen.width, 1);
    } else {
        // Landscape 모드일 때 실행할 스크립트
        setCookie('deviceWidth', window.screen.width, 1);
    }

    console.log('fnSetDeviceWidth');
}

function fnSetReturnUrl(pPAGE) {
    setCookie(pPAGE, window.location.href);
}

function fnPageClose() {
    var pathname = window.location.pathname.split('/');
    var pageID = '';
    for (i = 0; i < pathname.length; i++) {
        pageID = pathname[i].replace('.aspx', '');
    }

    if (fnIsEmpty(pageID) != true) {
        var returnUrl = getCookie(pageID);
        if (returnUrl != null || returnUrl != '') {
            window.location.href = returnUrl;
        }
        else {
            window.history.back();
        }
    }
    else {
        window.history.back();
    }
}

function fnShowSaveMsg() {
    try {
        var left = 0;
        var top = 0;
        var width = ucPopupSaveMsg.width;
        var height = ucPopupSaveMsg.height;
        var winWidth = window.innerWidth;
        var winHeight = window.innerHeight;

        if (winWidth <= width) {
            left = 0;
        }
        left = (winWidth - width) / 2;

        if (winHeight <= height) {
            top = 0;
        }
        top = (winHeight - height) / 2;

        ucPopupSaveMsg.ShowAtPos(left, top);
    }
    finally {

    }
}

function fnHideSaveMsg() {
    ucPopupSaveMsg.Hide();
}

// 접속기기 디바이스 체크 (모바일이면 true)
function isMobileDevice() {
    var mobileKeyWords = new Array('Android', 'iPhone', 'iPod', 'iPad', 'BlackBerry', 'Windows CE', 'Windows CE;', 'LG', 'MOT', 'SAMSUNG', 'SonyEricsson', 'Mobile', 'Symbian', 'Opera Mobi', 'Opera Mini', 'IEmobile');
    for (var word in mobileKeyWords) {
        if (navigator.userAgent.match(mobileKeyWords[word]) != null) {
            console.log('접속 디바이스 : ' + mobileKeyWords[word]);
            return true;
        }
    }

    // PC 판별
    var pcDevice = "win16|win32|win64|mac|macintel";
    if (navigator.platform) {
        if (pcDevice.indexOf(navigator.platform.toLowerCase()) < 0) {
            // 모바일
            console.log('접속 디바이스 : ' + '알 수 없는 모바일 기기');
            return true;
        } else {
            // PC
            console.log('접속 디바이스 : ' + 'PC');
            return false;
        }
    }

    console.log('접속 디바이스 : ' + '불분명한 기기');
    return true;
}

// 2021.07.19 윤기선 전각반각 변환기 , 히라가나 -> 카타카나 변환지원 (웹에 있는 자료 수정했음. 히라가나는 반각이 존재하지 않음)
(function () {
    let charsets = {
        latin: { halfRE: /[!-~]/g, fullRE: /[！-～]/g, delta: 0xFEE0 },
        hangul1: { halfRE: /[ﾡ-ﾾ]/g, fullRE: /[ᆨ-ᇂ]/g, delta: -0xEDF9 },
        hangul2: { halfRE: /[ￂ-ￜ]/g, fullRE: /[ᅡ-ᅵ]/g, delta: -0xEE61 },
        kana1: {
            delta: 0,
            half: "｡｢｣､･ｦｧｨｩｪｫｬｭｮｯｰｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜﾝﾞﾟ",
            full: "。「」、・ヲァィゥェォャュョッーアイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワン゛゜"
        },
        kana2: {
            delta: 0,
            kata: "。「」、・ヲァィゥェォャュョッーアイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワン゛゜ガギグゲゴザジズゼゾダヂヅデドバビブベボパピプペポ",
            hira: "。「」、・をぁぃぅぇぉゃゅょっーあいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわん゛゜がぎぐげござじずぜぞだぢづでどばびぶべぼぱぴぷぺぽ"
        },
        kana3: { // 탁음기호, 반탁음기호
            delta: 0,
            kataA1: "ガギグゲゴザジズゼゾダヂヅデドバビブベボ",
            kataA2: "カキクケコサシスセソタチツテトハヒフヘホ",
            kataB1: "パピプペポ",
            kataB2: "ハヒフヘホ"
        },
        extras: {
            delta: 0,
            half: "¢£¬¯¦¥₩\u0020|←↑→↓■°",
            full: "￠￡￢￣￤￥￦\u3000￨￩￪￫￬￭￮"
        }
    };
    let toFull = set => c => set.delta ?
        String.fromCharCode(c.charCodeAt(0) + set.delta) :
        [...set.full][[...set.half].indexOf(c)];
    let toHalf = set => c => set.delta ?         // 히라가나와 탁음은 변환 안됨
        String.fromCharCode(c.charCodeAt(0) - set.delta) :
        [...set.half][[...set.full].indexOf(c)];
    let toKata = set => c => set.delta ?
        String.fromCharCode(c.charCodeAt(0) - set.delta) :
        [...set.kata][[...set.hira].indexOf(c)];
    let toHira = set => c => set.delta ?
        String.fromCharCode(c.charCodeAt(0) - set.delta) :
        [...set.hira][[...set.kata].indexOf(c)];
    // 탁음기호, 반탁음기호
    let toKataADouble = set => c => set.delta ?
        String.fromCharCode(c.charCodeAt(0) - set.delta) :
        [...set.kataA2][[...set.kataA1].indexOf(c)] + '゛';
    let toKataBDouble = set => c => set.delta ?
        String.fromCharCode(c.charCodeAt(0) - set.delta) :
        [...set.kataB2][[...set.kataB1].indexOf(c)] + '゜';

    let re = (set, way) => set[way + "RE"] || new RegExp("[" + set[way] + "]", "g");
    let sets = Object.keys(charsets).map(i => charsets[i]);

    // 실제 사용하면 되는 메소드
    window.toFullWidth = str0 =>
        sets.reduce((str, set) => str.replace(re(set, "half"), toFull(set)), str0);
    window.toHalfWidth = str0 =>
        sets.reduce((str, set) => str.replace(re(set, "full"), toHalf(set)), str0);
    window.toKatakana = str0 =>
        sets.reduce((str, set) => str.replace(re(set, "hira"), toKata(set)), str0);
    window.toHiragana = str0 =>
        sets.reduce((str, set) => str.replace(re(set, "kata"), toHira(set)), str0);
    // [private] ガ -> ｶﾞ 탁음기호를 둘로 나누는 용
    let toKatakanaADouble = str0 =>
        sets.reduce((str, set) => str.replace(re(set, "kataA1"), toKataADouble(set)), str0);
    let toKatakanaBDouble = str0 =>
        sets.reduce((str, set) => str.replace(re(set, "kataB1"), toKataBDouble(set)), str0);
    // 히라가나,카타카나 전각 -> 카타카나 반각으로 변경시 아래 메소드 권장
    window.toHalfWidthKatakana = str0 => {
        return window.toHalfWidth(toKatakanaBDouble(toKatakanaADouble(window.toKatakana(str0))));
    }

})();

/*
 * 2021.06.02 윤기선
 * 
 * HTML공통 규격으로 지원되는 beforeunload 메소드가 아이폰의 사파리앱에서는 지원을 공식적으로 안함.
 * 대체되는 pagehide 이벤트로 시도해보았으나 페이지 나가는 것을 멈출 방법이 없는 등 제약사항이 많아서, 해당 기능은 지원 안하는 것으로 결정함.
 * 특히 해당 이벤트는 아이폰X 이후부터 지원을 안하기 시작하였는데, 광고 사이트 등에서 악용을 하여 애플에서 막은것으로 추측된다는 의견이 있음
 * 
 * beforeunload IOS 사파리에서 지원안한다는 출처
 * https://developer.mozilla.org/en-US/docs/Web/API/Window/beforeunload_event
 * 
 * (유저가 어떤 것을 작성 중에) 페이지 벗어날 시
 * 페이지를 나가시겠습니까? 메시지창을 띄우고 취소를 누르면 페이지에 남아있기 기능에 대해 활성화할지 말지.
 */

function isBeforeUnloadEnabled(event) {

    let onoff = false; // 해당 변수의 값을 false로 하면 페이지 벗어날 시 메시지 띄우지 않음

    return onoff;
}

// 변수 등 오브젝트의 문자열 이름을 실제 오브젝트로 반환해줌
function TextNameToObject(pName) {
    return Function('"use strict"; return (' + pName + ')')();
}