
// Hello World! example user script
// version 0.1 BETA!
// 2005-04-25
// Copyright (c) 2005, Mark Pilgrim
// Released under the GPL license
// http://www.gnu.org/copyleft/gpl.html
//
// --------------------------------------------------------------------
//
// This is a Greasemonkey user script.  To install it, you need
// Greasemonkey 0.3 or later: http://greasemonkey.mozdev.org/
// Then restart Firefox and revisit this script.
// Under Tools, there will be a new menu item to "Install User Script".
// Accept the default configuration and install.
//
// To uninstall, go to Tools/Manage User Scripts,
// select "Hello World", and click Uninstall.
//
// --------------------------------------------------------------------
//
// ==UserScript==
// @name          icfp
// @namespace     http://diveintomark.org/projects/greasemonkey/
// @description   example script to alert "Hello world!" on every page
// @include       http://nfa.imn.htwk-leipzig.de/recent_cars/*
// @exclude       http://diveintogreasemonkey.org/*
// @exclude       http://www.diveintogreasemonkey.org/*
// ==/UserScript==

var pres = document.getElementsByTagName("pre");

for (var p in pres)
    if (pres[p].innerHTML) {
        document.getElementById("G0").value = pres[p].innerHTML.match(/[0-9]+/);
        pres[p].innerHTML = pres[p].innerHTML.replace(/\(([0-9]*)/,
        '(<a href=\'http://icfpcontest.org/icfp10/instance/$1/solve/form?x=' +
         (Math.floor(Math.random() * 11) + 1) +
         '&y=' + (Math.floor(Math.random() * 11) + 1) + '\'>$1</a>');
    }
