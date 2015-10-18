
doSort = function () {
    $($(".cascade")).sortable({
        handle: "input",
        connectWith: ".cascade",
        cancel: "",
        revert: true,
        stop: function (event, ui) {
            divWrap()
        }
    });
};

divWrap = function () {
    if ($(".cascade").length)
        $("#sortableUL li").unwrap()

    var cascade = Math.floor(Math.floor($(document).width() - 61) / $("#sortableUL li").width()); // Odd row
    var start = 0, end = 0;
    while (start <= $(".ButtonStyle").length) {
        start = end;
        end = start + cascade;
        $("#sortableUL li").slice(start, end).wrapAll("<div class='cascade'> </div>");
        start = end;
        end = start + (cascade - 1);
        $("#sortableUL li").slice(start, end).wrapAll("<div style='margin-left:53px' class='cascade'> </div>");
    }
    doSort();
}

initStorage = function () {
    var obj, storage;

    for (var i = 0; i < JSONObj.length; i++) {
        obj = JSONObj[i];
        if ((storage = sessionStorage.getItem(obj.symbol)) === null) {
            sessionStorage.setItem(obj.symbol, obj.component);
        } else {
            sessionStorage.setItem(obj.symbol, storage.component + obj.component);
        }
    }
}

buttonPress = function () {
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.open("GET", "te-narai.org/女", false);
    xmlhttp.send(null);
    return xtmlhttp.responseText;
}

initButtons = function () {
    var btn;
    for (var i = 0; i < JSONObj2.length; i++) {
        btn = document.createElement("BUTTON");
        btn.innerHTML = JSONObj2[i].radical;
        btn.className = "ButtonStyle";
        btn.id = "button_" + i;
        $('#sortableUL').append(btn);
        $('#sortableUL').append("     ");
        document.getElementById(btn.id).addEventListener("click", buttonPress);
    }
};

function GetInformation(data) {
    document.getElementById('<%=CodeBehind.ClientID%>').value = data;
}

$(document).ready(function () {
    //initButtons();
    var rtime = new Date(1, 1, 2000, 12, 00, 00);
    var timeout = false;
    var delta = 200;
    $(window).resize(function () {
        rtime = new Date();
        if (timeout === false) {
            timeout = true;
            setTimeout(resizeend, delta);
        }
    });

    function resizeend() {
        if (new Date() - rtime < delta) {
            setTimeout(resizeend, delta);
        } else {
            timeout = false;
            divWrap();
        }
    }

    divWrap();
});
