/* 자바스크립트 모음
 * 
 * 블레이저는 _Host.cshtml 파일 body 내에 자바스크립트 함수를 넣고
 * await JS.InvokeVoidAsync("JS_test", x); 메소드를 부르는 형식으로 짜야함
 * .razor 내에 script 태그를 쓸 수 없기 때문임
 * 
 * @using Microsoft.JSInterop;
   @inject IJSRuntime JS
   해당 .razor 페이지에서 위의 주입도 해주어야 함.
 */

//window.onload = function () {

//};

window.JS_addEventListener_resize = (dotNetHelper) => {
    window.addEventListener('resize', function () {
        let lClientBrowserWidth = window.innerWidth;
        let lClientBrowserHeight = window.innerHeight;
        console.log('[main.js] resize width : ' + lClientBrowserWidth + ' height : ' + lClientBrowserHeight);
        dotNetHelper.invokeMethodAsync('ClientBrowserResized', lClientBrowserWidth, lClientBrowserHeight);
    });
};


//test
window.JS_videoJoin = (element, pSrc) => {
    element.setAttribute('allow', 'microphone; camera');
    //element.style.width = '100%';
    //element.style.height = $("#chatWindowSize").height();
    $(element).height($("#chatWindowSize").height() - 8);
    //element.hidden = false;

    $(element).attr('src', '' + pSrc);
};

// 받은 객체를 콘솔로그 출력
// async private Task test() 등의 메소드 내에서 await JS.InvokeVoidAsync("JS_log", ""); 사용
window.JS_log = (element) => console.log(element);

window.JS_alert = (element) => alert(element);

// 해당 객체(element)를 클릭하고 다시 원래 있던 포커스로 되돌림
window.JS_click = (element) => {
    var focusElement = document.activeElement;
    element.focus();
    element.click();
    focusElement.focus();
};

// 해당 객체의 가장 아랫단으로 스크롤 시킴
window.JS_focusGoBottom = (element) => {
    element.scrollTop = element.scrollHeight;
};

// 해당 객체의 Visible 유무
window.JS_setVisible = (pTarget, pBoolean) => {
    if (pBoolean == true) {
        $(pTarget).removeClass("d-none");
    }
    else {
        $(pTarget).addClass("d-none");
    }
};

// 클라이언트의 브라우저 크기 반환
window.JS_getClientWidthHeight = () => {
    return ('' + window.innerWidth + '|' + window.innerHeight);
};

// 해당 객체의 x와 y위치 반환. Left Top에서 얼마나 먼지 ex) "20|30" 등으로 반환
window.JS_getPosition = (pId) => {
    let lPosition = $(pId).position();
    return ('' + lPosition.left + '|' + lPosition.top);
};

// 화면스크롤 이동
window.JS_setScroll = (pLeft, pTop) => {
    $('html').scrollTop(pTop);
    $('html').scrollLeft(pLeft);
};
// 화면스크롤 좌우 이동
window.JS_setScrollLeft = (pLeft) => {
    $('html').scrollLeft(pLeft);
};
// 화면스크롤 상하 이동
window.JS_setScrollTop = (pTop) => {
    $('html').scrollTop(pTop);
};

window.JS_setCss = (pId, pKey, pValue) => {
    $(pId).css(pKey, pValue);
}

// 클래스 추가
window.JS_addClass = (pId, pClass) => {
    $(pId).addClass(pClass);
}

// 클래스 삭제
window.JS_removeClass = (pId, pClass) => {
    $(pId).removeClass(pClass);
}

// 현재 활성화된 Html 요소
window.JS_getId = () => {
    return document.activeElement.id;
}

// 해당 요소의 Width
window.JS_getWidth = (pId) => {
    return $(pId).width();
}

// 해당 요소의 Height
window.JS_getHeight = (pId) => {
    return $(pId).height();
}

window.JS_ProWindow_getDialogId = () => {
    let element = document.activeElement;
    let lResult = '';
    try {
        while (true) {
            if ((element == null) || (typeof element.id) == "undefined") { // 2022.01.04 
                console.log('[JS_ProWindow_getDialogId] 요소없음');
                return;
            }

            if (element.id.substr(0, 7) == 'dialog-') {
                lResult = element.id.substr(0, 43);
                break;
            }
            else if (element.nodeName == 'MAIN') {
                lResult = '';
                break;
            }
            else
                element = element.parentElement;
        }
    } catch (e) {
        console.log(e); // 2022.01.03  가끔 창 크기 조절하다 캐치로 빠짐
    }

    return lResult;
}

// [ProWindow] 해당 팝업 포커싱
window.JS_ProWindow_focus = () => {
    $('.e-dialog.e-lib.e-popup.e-control').removeClass('ProWindow-focus');

    let element = document.activeElement;
    let lFind = false;
    while (true) {
        if ((element == null) || (typeof element.id) == "undefined") { // 2022.01.04 
            console.log('[JS_ProWindow_focus] 요소없음');
            return;
        }

        if (element.id.length == 43 && element.id.substr(0, 7) == 'dialog-') {
            lResult = element.id.substr(0, 43);
            lFind = true;
            break;
        }
        else if (element.nodeName == 'MAIN') {
            lFind = false;
            break;
        }
        else
            element = element.parentElement;
    }

    if (lFind == true)
        $('#' + element.id + '.e-dialog.e-lib.e-popup.e-control').addClass('ProWindow-focus');
}

window.JS_ProWindow_getLocation = (pId) => {
    if (fnNVL(pId, '') == '')
        return '';

    let lResult = '';
    try {
        lResult += $(pId).outerWidth() + '|';
        lResult += $(pId).outerHeight() + '|';
        lResult += $(pId).offset().left + '|';
        lResult += $(pId).offset().top + '|';
        lResult += $(pId).css('z-index');
    } catch (e) {
        console.log(e); // 2022.01.14  창 닫은 후로 다른 창 위치이동시 캐치로 빠짐
    }
    return lResult;
}


window.JS_Collapse = (pButtonID, pElementID) => {
    let buttonID = '#' + pButtonID;
    let elementID = '#' + pElementID;

    if ($(elementID) !== null) {
        lName = $(elementID).attr('name');

        $('[name=' + lName + ']').removeClass('active');
        $(buttonID).addClass('active');

        $('[name=' + lName + ']').removeClass('show');
        $(elementID).addClass('show');
    }
}

window.JS_Collapse = (pButtonID, pElementID) => {
    let buttonID = '#' + pButtonID;
    let elementID = '#' + pElementID;

    if ($(elementID) !== null) {
        lName = $(elementID).attr('name');

        $('[name=' + lName + ']').removeClass('active');
        $(buttonID).addClass('active');

        $('[name=' + lName + ']').removeClass('show');
        $(elementID).addClass('show');
    }
}


window.JS_ListContentvisitedTitleTemp = () => {
    $('.ListContentvisitedTitleTemp').removeClass('ListContentvisitedTitleTemp');
}

window.JS_goToIframePro = (pUrl, pTitleId) => {
    $('#userIframe').attr('src', pUrl);
    $('#' + pTitleId).attr('class', 'ListContentvisitedTitleTemp');
}

window.JS_goToIframeLite = (pUrl) => {
    $('#userIframe').attr('src', pUrl);
}

// 에어 화면에서 글 눌렀을 때 팝업
window.JS_goToIframeAir = (pUrl, pTitleId) => {
    if (window.devicePixelRatio > 1) // 모바일인 경우
        //window.open(pUrl, "_blank");
        ;
    else {
        let popupWidth = 1500;
        let popupHeight = 800;
        let popupX = (window.screen.width / 2) - (popupWidth / 2);
        let popupY = (window.screen.height / 2) - (popupHeight / 2);

        window.open(pUrl, 'RGWebViewer_Air2'
            , 'width=' + popupWidth + ',height=' + popupHeight + ',left=' + popupX + ',top=' + popupY
            + 'toolbar=no,location=no,menubar=no,status=no,titlebar=no');
    }
    $('#' + pTitleId).attr('class', 'ListContentvisitedTitleTemp');
}

// 에어 화면 팝업
window.JS_goToAir = () => {
    if (window.devicePixelRatio > 1) // 모바일인 경우
        document.location.href = 'rg/userproairpage';
    else {
        let popupWidth = 350;
        let popupHeight = 800;
        let popupX = (window.screen.width / 2) - (popupWidth / 2);
        let popupY = (window.screen.height / 2) - (popupHeight / 2);

        window.open('rg/userproairpage', 'RGWebViewer_Air1'
            , 'width=' + popupWidth + ',height=' + popupHeight + ',left=' + popupX + ',top=' + popupY
            + 'toolbar=no,location=no,menubar=no,status=no,titlebar=no')
    }
}
// 에어 화면 열리면 스크롤 최하단으로 이동
window.JS_initAir = () => {
    window.scrollTo(0, document.body.scrollHeight);
}

// 글쓰기 눌렀을 때  pPopupOnoff: 팝업모드로 열지 / pFixedPCMode: 강제 PC모드로 설정
window.JS_goToContentWrite = (pUrl, pUrlMobile, pPopupOnoff, pFixedPCMode) => {
    let Url = "";
    if (window.devicePixelRatio > 1) // 모바일인 경우
        Url = pUrlMobile;
    else
        Url = pUrl;

    if (pFixedPCMode == true)   // 강제 PC모드가 설정되어 있는 경우
        Url = pUrl;

    if (pPopupOnoff == false) // 팝업모드 off (새탭모드)
        window.open(Url, "_blank");
    else {
        let popupWidth = 1500;
        let popupHeight = 800;
        let popupX = (window.screen.width / 2) - (popupWidth / 2);
        let popupY = (window.screen.height / 2) - (popupHeight / 2);

        window.open(Url, 'RGWebViewer_ContentWrite'
            , 'width=' + popupWidth + ',height=' + popupHeight + ',left=' + popupX + ',top=' + popupY
            + 'toolbar=no,location=no,menubar=no,status=no,titlebar=no');
    }
}


// 출처 : https://nomo.asia/402
String.prototype.hashCode = function () {
    var hash = 0,
        i, char;
    if (this.length === 0) return hash;
    for (i = 0; i < this.length; i++) {
        char = this.charCodeAt(i);
        hash = ((hash << 5) - hash) + char;
        hash & hash; // Convert to 32bit integer
    }
    return hash;
};

var random_color = {
    lib: {
        orange: "#FF4500", brownishorange: "#DAA520", darkgreen: "#008000", blue: "#0000FF", blueviolet: "#8a2be2", brown: "#a52a2a", cadetblue: "#5f9ea0", chocolate: "#d2691e", coral: "#ff7f50", cornflowerblue: "#6495ed", crimson: "#dc143c", darkblue: "#00008b", darkgoldenrod: "#b8860b", darkmagenta: "#8b008b", darkolivegreen: "#556b2f", darkorange: "#ff8c00", darkorchid: "#9932cc", darkred: "#8b0000", darksalmon: "#e9967a", darkslateblue: "#483d8b", darkslategray: "#2f4f4f", darkturquoise: "#00ced1", darkviolet: "#9400d3", deeppink: "#ff1493", dimgray: "#696969", dodgerblue: "#1e90ff", firebrick: "#b22222", forestgreen: "#228b22", grey: "#808080", hotpink: "#ff69b4", indianred: "#cd5c5c", indigo: "#4b0082", lightcoral: "#f08080", lightsalmon: "#ffa07a", lightseagreen: "#20b2aa", lightslategrey: "#778899", limegreen: "#32cd32", magenta: "magenta", mediumblue: "#0000cd", mediumorchid: "#ba55d3", mediumpurple: "#9370db", mediumseagreen: "#3cb371", mediumslateblue: "#7b68ee", mediumturquoise: "#48d1cc", mediumvioletred: "#c71585", midnightblue: "#191970", navy: "#000080", olive: "olive", olivedrab: "#6b8e23", orangered: "#ff4500", orchid: "#da70d6", pink: "#FF69B4", purple: "purple", red: "#FF0000", rosybrown: "#bc8f8f", royalblue: "#4169e1", saddlebrown: "#8b4513", salmon: "#fa8072", seagreen: "#2e8b57", sienna: "#a0522d", slateblue: "#6a5acd", slategrey: "#708090", steelblue: "#4682b4", tan: "#d2b48c", tomato: "#ff6347", violet: "#ee82ee",
    },
    random: function (str) {
        var hash, color_key;
        var colors_keys = Object.keys(this.lib);
        var colors_keys_length = colors_keys.length;

        // 입력 값이 없는 경우 임의의 랜덤 색상값 출력
        if (str === undefined) {
            hash = Math.floor((Math.random() * colors_keys_length) + 1); // random range: 0 - colors_keys_length
        }
        // 입력 값이 있는 경우, String 의 hash 에 따른 색상값 출력
        else {
            hash = str.hashCode();
            hash = ((hash % colors_keys_length) + colors_keys_length) % colors_keys_length; // range: 0 - colors_keys_length
        }

        color_key = colors_keys[hash];
        return { name: color_key, rgb: this.lib[color_key] };
    }
};

window.JS_Colorize = () => {
    $(".colorize").each(function (index, elem) {
        var temp_html = $(elem).html();
        var color_obj = random_color.random(temp_html);
        $(elem).css("color", color_obj.rgb).attr("colorize", color_obj.name);
    });
}

// 모바일 유무 판별
window.JS_UserAgent = () => {
    let currentOS;
    let agent = (/iphone|ipad|ipod|macintosh|android/i.test(navigator.userAgent.toLowerCase()));

    if (agent) {
        // 유저에이전트를 불러와서 OS를 구분합니다.
        let userAgent = navigator.userAgent.toLowerCase();

        if (userAgent.search("android") > -1) {
            currentOS = "android";
            mobileYN = true;
        }
        else if ((userAgent.search("iphone") > -1) || (userAgent.search("ipod") > -1)
            || (userAgent.search("ipad") > -1)) {
            currentOS = "ios";
        }
        else if (userAgent.search("macintosh") > -1) {
            currentOS = "macintosh";
        }
        else {
            currentOS = "other";
        }
    } else {
        // 모바일이 아닐 때
        currentOS = "pc";
    }

    return currentOS;
}


