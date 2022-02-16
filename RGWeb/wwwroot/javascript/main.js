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
            if ((element == null) || (typeof element.id) == "undefined") { // 2022.01.04 윤기선
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
        console.log(e); // 2022.01.03 윤기선 가끔 창 크기 조절하다 캐치로 빠짐
    }

    return lResult;
}

// [ProWindow] 해당 팝업 포커싱
window.JS_ProWindow_focus = () => {
    $('.e-dialog.e-lib.e-popup.e-control').removeClass('ProWindow-focus');

    let element = document.activeElement;
    let lFind = false;
    while (true) {
        if ((element == null) || (typeof element.id) == "undefined") { // 2022.01.04 윤기선
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
        console.log(e); // 2022.01.14 윤기선 창 닫은 후로 다른 창 위치이동시 캐치로 빠짐
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



window.JS_goToIframe = (pUrl, pTitleId) => {
    $('#userIframe').attr('src', pUrl);
    $('#' + pTitleId).attr('class', 'ListContentvisitedTitleTemp');
}

window.JS_ListContentvisitedTitleTemp = () => {
    $('.ListContentvisitedTitleTemp').removeClass('ListContentvisitedTitleTemp');
}

