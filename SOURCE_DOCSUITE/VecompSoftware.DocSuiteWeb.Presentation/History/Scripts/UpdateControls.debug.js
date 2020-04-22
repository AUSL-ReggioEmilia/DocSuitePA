//! UpdateControls.js
//! Copyright (c) Nikhil Kothari, 2007. All Rights Reserved.
//! http://www.nikhilk.net
//!
//! This product's copyrights are licensed under the Creative
//! Commons Attribution-ShareAlike (version 2.5).B
//! http://creativecommons.org/licenses/by-sa/2.5/
//!
//! You are free to:
//! - copy, distribute, display, and perform the work 
//! - make derivative works 
//! - make commercial use of the work 
//! as long as the work is itself not a server control offering of
//! any sort, under the following conditions:
//! Attribution. You must attribute the original work in your
//!              product or release.
//! Share Alike. If you alter, transform, or build upon this work,
//!              you may distribute the resulting work only under
//!              a license identical to this one.
//!


Type.registerNamespace('nStuff');

////////////////////////////////////////////////////////////////////////////////
// nStuff.AnimationType

nStuff.AnimationType = function() { };
nStuff.AnimationType.prototype = {
    crossFade: 0, 
    highlight: 1, 
    slideUp: 2, 
    slideDown: 3, 
    wipeLeft: 4, 
    wipeRight: 5
}
nStuff.AnimationType.registerEnum('nStuff.AnimationType', false);


////////////////////////////////////////////////////////////////////////////////
// nStuff.AnimatedUpdatePanelOptions

nStuff.$create_AnimatedUpdatePanelOptions = function nStuff_AnimatedUpdatePanelOptions() { return {}; }


////////////////////////////////////////////////////////////////////////////////
// nStuff.UpdateIndicatorOptions

nStuff.$create_UpdateIndicatorOptions = function nStuff_UpdateIndicatorOptions() { return {}; }


////////////////////////////////////////////////////////////////////////////////
// nStuff.UpdateActionOptions

nStuff.$create_UpdateActionOptions = function nStuff_UpdateActionOptions() { return {}; }


////////////////////////////////////////////////////////////////////////////////
// nStuff.ScrollOffset

nStuff.ScrollOffset = function() { };
nStuff.ScrollOffset.prototype = {
    top: 0, 
    bottom: 1, 
    middle: 2
}
nStuff.ScrollOffset.registerEnum('nStuff.ScrollOffset', false);


////////////////////////////////////////////////////////////////////////////////
// nStuff.UpdateHistoryOptions

nStuff.$create_UpdateHistoryOptions = function nStuff_UpdateHistoryOptions() { return {}; }


////////////////////////////////////////////////////////////////////////////////
// nStuff.AnimatedUpdatePanel

nStuff.AnimatedUpdatePanel = function nStuff_AnimatedUpdatePanel(element) {
    nStuff.AnimatedUpdatePanel.initializeBase(this, [ element ]);
}
nStuff.AnimatedUpdatePanel.prototype = {
    _animation$2: null,
    _pageLoadedHandler$2: null,
    _pageLoadingHandler$2: null,
    _updatePanel$2: null,
    _oldUpdatePanel$2: null,
    
    get_animation: function nStuff_AnimatedUpdatePanel$get_animation() {
        return null;
    },
    set_animation: function nStuff_AnimatedUpdatePanel$set_animation(value) {
        this._animation$2 = value;
        return value;
    },
    
    dispose: function nStuff_AnimatedUpdatePanel$dispose() {
        if (this._pageLoadedHandler$2) {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.remove_pageLoaded(this._pageLoadedHandler$2);
            prm.remove_pageLoading(this._pageLoadingHandler$2);
            this._pageLoadedHandler$2 = null;
            this._pageLoadingHandler$2 = null;
        }
        nStuff.AnimatedUpdatePanel.callBaseMethod(this, 'dispose');
    },
    
    initialize: function nStuff_AnimatedUpdatePanel$initialize() {
        nStuff.AnimatedUpdatePanel.callBaseMethod(this, 'initialize');
        this._pageLoadedHandler$2 = Function.createDelegate(this, this._onPageLoaded$2);
        this._pageLoadingHandler$2 = Function.createDelegate(this, this._onPageLoading$2);
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_pageLoaded(this._pageLoadedHandler$2);
        prm.add_pageLoading(this._pageLoadingHandler$2);
        var element = this.get_element();
        var childNodes = element.childNodes;
        for (var i = childNodes.length - 1; i >= 0; i--) {
            if (childNodes[i].nodeType === 1) {
                this._updatePanel$2 = childNodes[i];
                element.style.width = this._updatePanel$2.offsetWidth + 'px';
                element.style.height = this._updatePanel$2.offsetHeight + 'px';
                var updatePanelStyle = this._updatePanel$2.style;
                updatePanelStyle.position = 'absolute';
                updatePanelStyle.left = '0px';
                updatePanelStyle.top = '0px';
                break;
            }
        }
    },
    
    _onPageLoaded$2: function nStuff_AnimatedUpdatePanel$_onPageLoaded$2(sender, e) {
        if (!Array.contains(e.get_panelsUpdated(), this._updatePanel$2)) {
            return;
        }
        var element = this.get_element();
        element.style.width = this._updatePanel$2.offsetWidth + 'px';
        element.style.height = this._updatePanel$2.offsetHeight + 'px';
        if (this._animation$2.type === nStuff.AnimationType.highlight) {
            var overlay = document.createElement('div');
            overlay.className = this._animation$2.highlightCssClass;
            var overlayStyle = overlay.style;
            overlayStyle.display = 'none';
            overlayStyle.position = 'absolute';
            overlayStyle.top = '0px';
            overlayStyle.left = '0px';
            overlayStyle.width = element.offsetWidth + 'px';
            overlayStyle.height = element.offsetHeight + 'px';
            element.appendChild(overlay);
            var effect = new nStuff._highlightOverlayEffect(overlay, this._animation$2.duration);
            effect.play(null);
        }
        else {
            this._oldUpdatePanel$2.style.width = this._updatePanel$2.offsetWidth + 'px';
            this._oldUpdatePanel$2.style.height = this._updatePanel$2.offsetHeight + 'px';
            var updatePanelStyle = this._updatePanel$2.style;
            updatePanelStyle.position = 'absolute';
            updatePanelStyle.left = '0px';
            updatePanelStyle.top = '0px';
            if (this._animation$2.type === nStuff.AnimationType.crossFade) {
                var effect = new nStuff._crossFadeEffect(element, this._animation$2.duration);
                effect.playEffect(this._oldUpdatePanel$2, this._updatePanel$2);
            }
            else {
                var effect = new nStuff._slideOrWipeEffect(element, this._animation$2.duration);
                effect.playEffect(this._animation$2.type, this._oldUpdatePanel$2, this._updatePanel$2);
            }
        }
    },
    
    _onPageLoading$2: function nStuff_AnimatedUpdatePanel$_onPageLoading$2(sender, e) {
        if (!Array.contains(e.get_panelsUpdating(), this._updatePanel$2)) {
            return;
        }
        if (this._animation$2.type !== nStuff.AnimationType.highlight) {
            this._oldUpdatePanel$2 = this._updatePanel$2;
            this._updatePanel$2 = document.createElement(this._oldUpdatePanel$2.tagName);
            this._updatePanel$2.className = this._oldUpdatePanel$2.className;
            this._updatePanel$2.id = this._oldUpdatePanel$2.id;
            this._updatePanel$2.style.visibility = 'hidden';
            this._oldUpdatePanel$2.id = '';
            this._oldUpdatePanel$2.style.overflow = 'hidden';
            var oldHTML = this._oldUpdatePanel$2.innerHTML;
            sender._destroyTree(this._oldUpdatePanel$2);
            this._oldUpdatePanel$2.innerHTML = oldHTML;
            this.get_element().appendChild(this._updatePanel$2);
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// nStuff.UpdateIndicator

nStuff.UpdateIndicator = function nStuff_UpdateIndicator(element) {
    nStuff.UpdateIndicator.initializeBase(this, [ element ]);
}
nStuff.UpdateIndicator.prototype = {
    _options$2: null,
    
    get_options: function nStuff_UpdateIndicator$get_options() {
        return this._options$2;
    },
    set_options: function nStuff_UpdateIndicator$set_options(value) {
        this._options$2 = value;
        return value;
    },
    
    dispose: function nStuff_UpdateIndicator$dispose() {
        nStuff.UpdateIndicator.callBaseMethod(this, 'dispose');
    },
    
    initialize: function nStuff_UpdateIndicator$initialize() {
        nStuff.UpdateIndicator.callBaseMethod(this, 'initialize');
        var element = this.get_element();
        var location = Sys.UI.DomElement.getLocation(element);
        var offset = 0;
        if (this._options$2.scrollOffset === nStuff.ScrollOffset.bottom) {
            offset = element.offsetHeight;
        }
        else if (this._options$2.scrollOffset === nStuff.ScrollOffset.middle) {
            offset = Math.floor(element.offsetHeight / 2);
        }
        var scroll = new nStuff._scrollEffect(document.body, this._options$2.scrollDuration);
        scroll.playEffect(location.x, location.y + offset, Function.createDelegate(this, this._onScrollCompleted$2));
    },
    
    _onHighlightCompleted$2: function nStuff_UpdateIndicator$_onHighlightCompleted$2() {
        this.dispose();
    },
    
    _onScrollCompleted$2: function nStuff_UpdateIndicator$_onScrollCompleted$2() {
        if (this._options$2.highlightDuration) {
            var element = this.get_element();
            var overlay = document.createElement('div');
            overlay.className = this._options$2.highlightCssClass;
            var overlayStyle = overlay.style;
            overlayStyle.display = 'none';
            overlayStyle.position = 'absolute';
            overlayStyle.top = '0px';
            overlayStyle.left = '0px';
            overlayStyle.width = element.offsetWidth + 'px';
            overlayStyle.height = element.offsetHeight + 'px';
            element.appendChild(overlay);
            var effect = new nStuff._highlightOverlayEffect(overlay, this._options$2.highlightDuration);
            effect.play(Function.createDelegate(this, this._onHighlightCompleted$2));
        }
        else {
            this.dispose();
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// nStuff._scrollEffect

nStuff._scrollEffect = function nStuff__scrollEffect(domElement, duration) {
    nStuff._scrollEffect.initializeBase(this, [ domElement, duration ]);
    this.set_easingFunction(Function.createDelegate(null, nStuff.Glitz.TimedAnimation.easeInOut));
}
nStuff._scrollEffect.prototype = {
    _offsetX$2: 0,
    _offsetY$2: 0,
    _scrollXDelta$2: 0,
    _scrollYDelta$2: 0,
    
    performTweening: function nStuff__scrollEffect$performTweening(frame) {
        var scrollX = Math.ceil(frame * this._scrollXDelta$2) + this._offsetX$2;
        var scrollY = Math.ceil(frame * this._scrollYDelta$2) + this._offsetY$2;
        window.scrollTo(scrollX, scrollY);
    },
    
    playEffect: function nStuff__scrollEffect$playEffect(scrollToX, scrollToY, completedCallback) {
        this._offsetX$2 = document.body.scrollLeft;
        this._offsetY$2 = document.body.scrollTop;
        this._scrollXDelta$2 = scrollToX - this._offsetX$2;
        this._scrollYDelta$2 = scrollToY - this._offsetY$2;
        this.play(completedCallback);
    }
}


////////////////////////////////////////////////////////////////////////////////
// nStuff.UpdateAction

nStuff.UpdateAction = function nStuff_UpdateAction() {
    nStuff.UpdateAction.initializeBase(this);
}
nStuff.UpdateAction.prototype = {
    _appLoadHandler$1: null,
    _beginRequestHandler$1: null,
    _endRequestHandler$1: null,
    _options$1: null,
    _updateOptions$1: null,
    _scrollLeft$1: 0,
    _scrollTop$1: 0,
    
    get_options: function nStuff_UpdateAction$get_options() {
        return this._options$1;
    },
    set_options: function nStuff_UpdateAction$set_options(value) {
        this._options$1 = value;
        return value;
    },
    
    dispose: function nStuff_UpdateAction$dispose() {
        if (this._endRequestHandler$1) {
            Sys.WebForms.PageRequestManager.getInstance().remove_endRequest(this._endRequestHandler$1);
            this._endRequestHandler$1 = null;
        }
        if (this._beginRequestHandler$1) {
            Sys.WebForms.PageRequestManager.getInstance().remove_beginRequest(this._beginRequestHandler$1);
            this._beginRequestHandler$1 = null;
        }
        nStuff.UpdateAction.callBaseMethod(this, 'dispose');
    },
    
    initialize: function nStuff_UpdateAction$initialize() {
        nStuff.UpdateAction.callBaseMethod(this, 'initialize');
        this._appLoadHandler$1 = Function.createDelegate(this, this._onAppLoad$1);
        Sys.Application.add_load(this._appLoadHandler$1);
    },
    
    _onAppLoad$1: function nStuff_UpdateAction$_onAppLoad$1(sender, e) {
        Sys.Application.remove_load(this._appLoadHandler$1);
        this._appLoadHandler$1 = null;
        this._endRequestHandler$1 = Function.createDelegate(this, this._onPageRequestManagerEndRequest$1);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(this._endRequestHandler$1);
        this._beginRequestHandler$1 = Function.createDelegate(this, this._onPageRequestManagerBeginRequest$1);
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(this._beginRequestHandler$1);
        this._performActionsCore$1(this._options$1, false);
    },
    
    _onPageRequestManagerBeginRequest$1: function nStuff_UpdateAction$_onPageRequestManagerBeginRequest$1(sender, e) {
        Sys.WebForms.PageRequestManager.getInstance()._scrollPosition = null;
        Sys.WebForms.PageRequestManager.getInstance()._controlIDToFocus = null;
        if (this._options$1.maintainScrollPosition) {
            this._scrollLeft$1 = document.body.scrollLeft;
            this._scrollTop$1 = document.body.scrollTop;
        }
    },
    
    _onPageRequestManagerEndRequest$1: function nStuff_UpdateAction$_onPageRequestManagerEndRequest$1(sender, e) {
        this._updateOptions$1 = e.get_dataItems()[this._options$1.dataID];
        if (this._updateOptions$1) {
            window.setTimeout(Function.createDelegate(this, this._performActions$1), 100);
        }
    },
    
    _performActions$1: function nStuff_UpdateAction$_performActions$1() {
        this._performActionsCore$1(this._updateOptions$1, true);
    },
    
    _performActionsCore$1: function nStuff_UpdateAction$_performActionsCore$1(options, update) {
        if (update) {
            if (options.scrollToID) {
                var scrollToElement = $get(options.scrollToID);
                if (scrollToElement) {
                    var location = Sys.UI.DomElement.getLocation(scrollToElement);
                    var offset = 0;
                    if (options.scrollOffset === nStuff.ScrollOffset.bottom) {
                        offset = scrollToElement.offsetHeight;
                    }
                    else if (options.scrollOffset === nStuff.ScrollOffset.middle) {
                        offset = Math.floor(scrollToElement.offsetHeight / 2);
                    }
                    var scroll = new nStuff._scrollEffect(document.body, 1000);
                    scroll.playEffect(location.x, location.y + offset, null);
                }
            }
            else if (options.maintainScrollPosition) {
                window.scrollTo(this._scrollLeft$1, this._scrollTop$1);
            }
        }
        if (options.message) {
            alert(options.message);
        }
        if (options.focusID) {
            window.WebForm_AutoFocus(options.focusID);
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// nStuff._slideOrWipeEffect

nStuff._slideOrWipeEffect = function nStuff__slideOrWipeEffect(domElement, duration) {
    nStuff._slideOrWipeEffect.initializeBase(this, [ domElement, duration ]);
    this.set_easingFunction(Function.createDelegate(null, nStuff.Glitz.TimedAnimation.easeInOut));
}
nStuff._slideOrWipeEffect.prototype = {
    _animation$2: 0,
    _oldContent$2: null,
    _newContent$2: null,
    _height$2: 0,
    _width$2: 0,
    
    performCleanup: function nStuff__slideOrWipeEffect$performCleanup() {
        this._oldContent$2.parentNode.removeChild(this._oldContent$2);
        this._newContent$2.style.top = '0px';
        this._newContent$2.style.left = '0px';
        this._newContent$2.style.height = this._height$2 + 'px';
        this._newContent$2.style.overflow = 'visible';
        this._newContent$2.style.clip = 'rect(auto auto auto auto)';
    },
    
    performSetup: function nStuff__slideOrWipeEffect$performSetup() {
        this._height$2 = this.get_domElement().offsetHeight;
        this._width$2 = this.get_domElement().offsetWidth;
        this._newContent$2.style.visibility = 'visible';
        this._newContent$2.style.overflow = 'hidden';
        switch (this._animation$2) {
            case nStuff.AnimationType.slideUp:
                this._newContent$2.style.top = this._height$2 + 'px';
                this._newContent$2.style.height = '0px';
                break;
            case nStuff.AnimationType.slideDown:
                this._newContent$2.style.height = this._height$2 + 'px';
                this._newContent$2.style.clip = 'rect(auto auto 0px auto)';
                break;
            case nStuff.AnimationType.wipeLeft:
                this._newContent$2.style.height = this._height$2 + 'px';
                this._newContent$2.style.clip = String.format('rect(auto auto auto {0}px)', this._width$2);
                break;
            case nStuff.AnimationType.wipeRight:
                this._newContent$2.style.height = this._height$2 + 'px';
                this._newContent$2.style.clip = 'rect(auto 0px auto auto)';
                break;
        }
    },
    
    performTweening: function nStuff__slideOrWipeEffect$performTweening(frame) {
        switch (this._animation$2) {
            case nStuff.AnimationType.slideUp:
                var yOffset = Math.floor(this._height$2 * (1 - frame));
                this._newContent$2.style.top = yOffset + 'px';
                this._newContent$2.style.height = (this._height$2 - yOffset) + 'px';
                break;
            case nStuff.AnimationType.slideDown:
                this._newContent$2.style.clip = String.format('rect(auto auto {0}px auto)', Math.floor(this._height$2 * frame));
                break;
            case nStuff.AnimationType.wipeLeft:
                var xOffset = Math.floor(this._width$2 * (1 - frame));
                break;
            case nStuff.AnimationType.wipeRight:
                this._newContent$2.style.clip = String.format('rect(auto {0}px auto auto)', Math.floor(this._width$2 * frame));
                break;
        }
    },
    
    playEffect: function nStuff__slideOrWipeEffect$playEffect(animation, oldContent, newContent) {
        this._animation$2 = animation;
        this._oldContent$2 = oldContent;
        this._newContent$2 = newContent;
        this.play(null);
    }
}


////////////////////////////////////////////////////////////////////////////////
// nStuff._crossFadeEffect

nStuff._crossFadeEffect = function nStuff__crossFadeEffect(domElement, duration) {
    nStuff._crossFadeEffect.initializeBase(this, [ domElement, duration ]);
    this.set_easingFunction(Function.createDelegate(null, nStuff.Glitz.TimedAnimation.easeInOut));
}
nStuff._crossFadeEffect.prototype = {
    _oldContent$2: null,
    _newContent$2: null,
    
    performCleanup: function nStuff__crossFadeEffect$performCleanup() {
        this._oldContent$2.parentNode.removeChild(this._oldContent$2);
    },
    
    performSetup: function nStuff__crossFadeEffect$performSetup() {
        this._setOpacity$2(this._newContent$2, 0);
        this._newContent$2.style.visibility = 'visible';
        this._newContent$2.style.filter = '';
    },
    
    performTweening: function nStuff__crossFadeEffect$performTweening(frame) {
        this._setOpacity$2(this._newContent$2, frame);
        this._setOpacity$2(this._oldContent$2, 1 - frame);
    },
    
    playEffect: function nStuff__crossFadeEffect$playEffect(oldContent, newContent) {
        this._oldContent$2 = oldContent;
        this._newContent$2 = newContent;
        this.play(null);
    },
    
    _setOpacity$2: function nStuff__crossFadeEffect$_setOpacity$2(element, opacity) {
        if (window.navigator.userAgent.indexOf('MSIE') >= 0) {
            element.style.filter = 'alpha(opacity=' + (opacity * 100) + ')';
        }
        else {
            element.style.opacity = opacity.toString();
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// nStuff._highlightOverlayEffect

nStuff._highlightOverlayEffect = function nStuff__highlightOverlayEffect(domElement, duration) {
    nStuff._highlightOverlayEffect.initializeBase(this, [ domElement, duration ]);
}
nStuff._highlightOverlayEffect.prototype = {
    
    performCleanup: function nStuff__highlightOverlayEffect$performCleanup() {
        this.get_domElement().parentNode.removeChild(this.get_domElement());
    },
    
    performSetup: function nStuff__highlightOverlayEffect$performSetup() {
        this._setOpacity$2(0);
        this.get_domElement().style.display = '';
    },
    
    performTweening: function nStuff__highlightOverlayEffect$performTweening(frame) {
        this._setOpacity$2(Math.sin(frame * Math.PI) * 0.75);
    },
    
    playEffect: function nStuff__highlightOverlayEffect$playEffect() {
        this.play(null);
    },
    
    _setOpacity$2: function nStuff__highlightOverlayEffect$_setOpacity$2(opacity) {
        if (window.navigator.userAgent.indexOf('MSIE') >= 0) {
            this.get_domElement().style.filter = 'alpha(opacity=' + (opacity * 100) + ')';
        }
        else {
            this.get_domElement().style.opacity = opacity.toString();
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// nStuff.UpdateHistory

nStuff.UpdateHistory = function nStuff_UpdateHistory() {
    nStuff.UpdateHistory.initializeBase(this);
}
nStuff.UpdateHistory.prototype = {
    _timerCookie$1: 0,
    _timerHandler$1: null,
    _iframeLoadHandler$1: null,
    _appLoadHandler$1: null,
    _endRequestHandler$1: null,
    _ignoreIFrame$1: false,
    _ignoreTimer$1: false,
    _historyFrame$1: null,
    _emptyPageURL$1: null,
    _dataID$1: null,
    _postbackID$1: null,
    _currentEntry$1: null,
    
    get_options: function nStuff_UpdateHistory$get_options() {
        return null;
    },
    set_options: function nStuff_UpdateHistory$set_options(value) {
        this._currentEntry$1 = (value.initialEntry) ? value.initialEntry : '';
        this._dataID$1 = value.dataID;
        this._postbackID$1 = value.postbackID;
        return value;
    },
    
    _addEntry$1: function nStuff_UpdateHistory$_addEntry$1(entry) {
        this._ignoreTimer$1 = true;
        if (this._historyFrame$1) {
            this._ignoreIFrame$1 = true;
            this._historyFrame$1.src = this._emptyPageURL$1 + entry;
        }
        else {
            this._setEntry$1(entry);
        }
    },
    
    dispose: function nStuff_UpdateHistory$dispose() {
        if (this._historyFrame$1) {
            this._historyFrame$1.detachEvent('onload', this._iframeLoadHandler$1);
            this._historyFrame$1 = null;
        }
        if (this._timerCookie$1) {
            window.clearTimeout(this._timerCookie$1);
            this._timerCookie$1 = 0;
        }
        if (this._endRequestHandler$1) {
            Sys.WebForms.PageRequestManager.getInstance().remove_endRequest(this._endRequestHandler$1);
            this._endRequestHandler$1 = null;
        }
        nStuff.UpdateHistory.callBaseMethod(this, 'dispose');
    },
    
    _getEntry$1: function nStuff_UpdateHistory$_getEntry$1() {
        var entry = window.location.hash;
        if ((entry.length >= 1) && (entry.charAt(0) === '#')) {
            entry = entry.substr(1);
        }
        return entry;
    },
    
    initialize: function nStuff_UpdateHistory$initialize() {
        nStuff.UpdateHistory.callBaseMethod(this, 'initialize');
        this._appLoadHandler$1 = Function.createDelegate(this, this._onAppLoad$1);
        Sys.Application.add_load(this._appLoadHandler$1);
    },
    
    _navigate$1: function nStuff_UpdateHistory$_navigate$1(entry) {
        __doPostBack(this._postbackID$1, entry);
    },
    
    _onAppLoad$1: function nStuff_UpdateHistory$_onAppLoad$1(sender, e) {
        Sys.Application.remove_load(this._appLoadHandler$1);
        this._appLoadHandler$1 = null;
        this._endRequestHandler$1 = Function.createDelegate(this, this._onPageRequestManagerEndRequest$1);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(this._endRequestHandler$1);
        if (window.navigator.userAgent.indexOf('MSIE') >= 0) {
            this._historyFrame$1 = $get('__historyFrame');
            this._emptyPageURL$1 = this._historyFrame$1.src + '?';
            this._iframeLoadHandler$1 = Function.createDelegate(this, this._onIFrameLoad$1);
            this._historyFrame$1.attachEvent('onload', this._iframeLoadHandler$1);
        }
        this._timerHandler$1 = Function.createDelegate(this, this._onTick$1);
        this._timerCookie$1 = window.setTimeout(this._timerHandler$1, 100);
        var loadedEntry = this._getEntry$1();
        if (loadedEntry !== this._currentEntry$1) {
            this._currentEntry$1 = loadedEntry;
            this._navigate$1(loadedEntry);
        }
    },
    
    _onIFrameLoad$1: function nStuff_UpdateHistory$_onIFrameLoad$1() {
        var entry = this._historyFrame$1.contentWindow.location.search;
        if ((entry.length >= 1) && (entry.charAt(0) === '?')) {
            entry = entry.substr(1);
        }
        this._setEntry$1(entry);
        if (this._ignoreIFrame$1) {
            this._ignoreIFrame$1 = false;
            return;
        }
        this._navigate$1(entry);
    },
    
    _onPageRequestManagerEndRequest$1: function nStuff_UpdateHistory$_onPageRequestManagerEndRequest$1(sender, e) {
        var entry = e.get_dataItems()[this._dataID$1];
        if (entry) {
            this._addEntry$1(entry);
        }
    },
    
    _onTick$1: function nStuff_UpdateHistory$_onTick$1() {
        this._timerCookie$1 = 0;
        var entry = this._getEntry$1();
        if (entry !== this._currentEntry$1) {
            if (!this._ignoreTimer$1) {
                this._currentEntry$1 = entry;
                this._navigate$1(entry);
            }
        }
        else {
            this._ignoreTimer$1 = false;
        }
        this._timerCookie$1 = window.setTimeout(this._timerHandler$1, 100);
    },
    
    _setEntry$1: function nStuff_UpdateHistory$_setEntry$1(entry) {
        this._currentEntry$1 = entry;
        window.location.hash = entry;
    }
}


Type.registerNamespace('nStuff.Glitz');

////////////////////////////////////////////////////////////////////////////////
// nStuff.Glitz.AnimationStopState

nStuff.Glitz.AnimationStopState = function() { };
nStuff.Glitz.AnimationStopState.prototype = {
    complete: 0, 
    abort: 1, 
    revert: 2
}
nStuff.Glitz.AnimationStopState.registerEnum('nStuff.Glitz.AnimationStopState', false);


////////////////////////////////////////////////////////////////////////////////
// nStuff.Glitz.Animation

nStuff.Glitz.Animation = function nStuff_Glitz_Animation(domElement) {
    if (!domElement) {
        domElement = document.documentElement;
    }
    this._domElement = domElement;
    Sys.Application.registerDisposableObject(this);
}
nStuff.Glitz.Animation.prototype = {
    _domElement: null,
    _completed: false,
    _isPlaying: false,
    _completedCallback: null,
    
    get_completed: function nStuff_Glitz_Animation$get_completed() {
        return this._completed;
    },
    
    get_domElement: function nStuff_Glitz_Animation$get_domElement() {
        return this._domElement;
    },
    
    get_isPlaying: function nStuff_Glitz_Animation$get_isPlaying() {
        return this._isPlaying;
    },
    
    dispose: function nStuff_Glitz_Animation$dispose() {
        if (this._isPlaying) {
            this.stop(nStuff.Glitz.AnimationStopState.abort);
        }
        if (this._domElement) {
            this._domElement = null;
            Sys.Application.unregisterDisposableObject(this);
        }
    },
    
    _onPlay: function nStuff_Glitz_Animation$_onPlay(reversed) {
        this.performSetup();
        this._isPlaying = true;
        this.playCore();
    },
    
    _onStop: function nStuff_Glitz_Animation$_onStop(completed, stopState) {
        this.stopCore(completed, stopState);
        this._completed = completed;
        this._isPlaying = false;
        this.performCleanup();
        if (completed && (this._completedCallback)) {
            this._completedCallback();
        }
    },
    
    _onProgress: function nStuff_Glitz_Animation$_onProgress(timeStamp) {
        return this.progressCore(timeStamp);
    },
    
    performCleanup: function nStuff_Glitz_Animation$performCleanup() {
    },
    
    performSetup: function nStuff_Glitz_Animation$performSetup() {
    },
    
    play: function nStuff_Glitz_Animation$play(completedCallback) {
        Sys.Debug.assert(!this.get_isPlaying());
        this._completed = false;
        this._completedCallback = completedCallback;
        nStuff.Glitz.AnimationManager._play(this, this._domElement);
    },
    
    stop: function nStuff_Glitz_Animation$stop(stopState) {
        Sys.Debug.assert(this.get_isPlaying());
        nStuff.Glitz.AnimationManager._stop(this, stopState);
    }
}


////////////////////////////////////////////////////////////////////////////////
// nStuff.Glitz.AnimationManager

nStuff.Glitz.AnimationManager = function nStuff_Glitz_AnimationManager() {
}
nStuff.Glitz.AnimationManager.get_FPS = function nStuff_Glitz_AnimationManager$get_FPS() {
    return nStuff.Glitz.AnimationManager._fps;
}
nStuff.Glitz.AnimationManager.set_FPS = function nStuff_Glitz_AnimationManager$set_FPS(value) {
    Sys.Debug.assert((value > 0) && (value <= 100));
    nStuff.Glitz.AnimationManager._fps = value;
    return value;
}
nStuff.Glitz.AnimationManager._onTick = function nStuff_Glitz_AnimationManager$_onTick() {
    nStuff.Glitz.AnimationManager._timerCookie = 0;
    if (!nStuff.Glitz.AnimationManager._activeAnimations.length) {
        return;
    }
    var timeStamp = (new Date()).getTime();
    var currentAnimations = nStuff.Glitz.AnimationManager._activeAnimations;
    var newAnimations = [];
    nStuff.Glitz.AnimationManager._activeAnimations = null;
    for (var i = 0; i < currentAnimations.length; i++) {
        var animation = currentAnimations[i];
        var completed = animation._onProgress(timeStamp);
        if (completed) {
            animation._onStop(true, nStuff.Glitz.AnimationStopState.complete);
        }
        else {
            Array.add(newAnimations, animation);
        }
    }
    if (newAnimations.length) {
        nStuff.Glitz.AnimationManager._activeAnimations = newAnimations;
        if (!nStuff.Glitz.AnimationManager._timerCookie) {
            nStuff.Glitz.AnimationManager._timerCookie = window.setTimeout(Function.createDelegate(null, nStuff.Glitz.AnimationManager._onTick), 1000 / nStuff.Glitz.AnimationManager._fps);
        }
    }
}
nStuff.Glitz.AnimationManager._play = function nStuff_Glitz_AnimationManager$_play(animation, domElement) {
    if (!nStuff.Glitz.AnimationManager._activeAnimations) {
        nStuff.Glitz.AnimationManager._activeAnimations = [];
    }
    Array.add(nStuff.Glitz.AnimationManager._activeAnimations, animation);
    animation._onPlay(false);
    if (!nStuff.Glitz.AnimationManager._timerCookie) {
        nStuff.Glitz.AnimationManager._timerCookie = window.setTimeout(Function.createDelegate(null, nStuff.Glitz.AnimationManager._onTick), 1000 / nStuff.Glitz.AnimationManager._fps);
    }
}
nStuff.Glitz.AnimationManager._stop = function nStuff_Glitz_AnimationManager$_stop(animation, stopState) {
    Sys.Debug.assert(nStuff.Glitz.AnimationManager._activeAnimations);
    animation._onStop(false, stopState);
    Array.remove(nStuff.Glitz.AnimationManager._activeAnimations, animation);
}


////////////////////////////////////////////////////////////////////////////////
// nStuff.Glitz.TimedAnimation

nStuff.Glitz.TimedAnimation = function nStuff_Glitz_TimedAnimation(domElement, duration) {
    nStuff.Glitz.TimedAnimation.initializeBase(this, [ domElement ]);
    Sys.Debug.assert(duration > 0);
    this._duration$1 = duration;
}
nStuff.Glitz.TimedAnimation.easeIn = function nStuff_Glitz_TimedAnimation$easeIn(t) {
    return t * t;
}
nStuff.Glitz.TimedAnimation.easeInOut = function nStuff_Glitz_TimedAnimation$easeInOut(t) {
    t = t * 2;
    if (t < 1) {
        return t * t / 2;
    }
    return -((--t) * (t - 2) - 1) / 2;
}
nStuff.Glitz.TimedAnimation.easeOut = function nStuff_Glitz_TimedAnimation$easeOut(t) {
    return -t * (t - 2);
}
nStuff.Glitz.TimedAnimation.prototype = {
    _duration$1: 0,
    _easingFunction$1: null,
    _startTimeStamp$1: 0,
    
    get_duration: function nStuff_Glitz_TimedAnimation$get_duration() {
        return this._duration$1;
    },
    set_duration: function nStuff_Glitz_TimedAnimation$set_duration(value) {
        Sys.Debug.assert(!this.get_isPlaying());
        Sys.Debug.assert(this._duration$1 >= 0);
        this._duration$1 = value;
        return value;
    },
    
    get_easingFunction: function nStuff_Glitz_TimedAnimation$get_easingFunction() {
        return this._easingFunction$1;
    },
    set_easingFunction: function nStuff_Glitz_TimedAnimation$set_easingFunction(value) {
        Sys.Debug.assert(!this.get_isPlaying());
        this._easingFunction$1 = value;
        return value;
    },
    
    playCore: function nStuff_Glitz_TimedAnimation$playCore() {
        this._startTimeStamp$1 = (new Date()).getTime();
        this.progressCore(this._startTimeStamp$1);
    },
    
    progressCore: function nStuff_Glitz_TimedAnimation$progressCore(timeStamp) {
        var frame = 0;
        var completed = false;
        frame = (timeStamp - this._startTimeStamp$1) / this._duration$1;
        completed = (frame >= 1);
        frame = Math.min(1, frame);
        if ((!completed) && (this._easingFunction$1)) {
            frame = this._easingFunction$1(frame);
        }
        this.performTweening(frame);
        return completed;
    },
    
    stopCore: function nStuff_Glitz_TimedAnimation$stopCore(completed, stopState) {
        if (!completed) {
            if (stopState === nStuff.Glitz.AnimationStopState.complete) {
                this.performTweening(1);
            }
            else if (stopState === nStuff.Glitz.AnimationStopState.revert) {
                this.performTweening(0);
            }
        }
    }
}


nStuff.AnimatedUpdatePanel.registerClass('nStuff.AnimatedUpdatePanel', Sys.UI.Control);
nStuff.UpdateIndicator.registerClass('nStuff.UpdateIndicator', Sys.UI.Behavior);
nStuff.Glitz.Animation.registerClass('nStuff.Glitz.Animation', null, Sys.IDisposable);
nStuff.Glitz.TimedAnimation.registerClass('nStuff.Glitz.TimedAnimation', nStuff.Glitz.Animation);
nStuff._scrollEffect.registerClass('nStuff._scrollEffect', nStuff.Glitz.TimedAnimation);
nStuff.UpdateAction.registerClass('nStuff.UpdateAction', Sys.Component);
nStuff._slideOrWipeEffect.registerClass('nStuff._slideOrWipeEffect', nStuff.Glitz.TimedAnimation);
nStuff._crossFadeEffect.registerClass('nStuff._crossFadeEffect', nStuff.Glitz.TimedAnimation);
nStuff._highlightOverlayEffect.registerClass('nStuff._highlightOverlayEffect', nStuff.Glitz.TimedAnimation);
nStuff.UpdateHistory.registerClass('nStuff.UpdateHistory', Sys.Component);
nStuff.Glitz.AnimationManager.registerClass('nStuff.Glitz.AnimationManager');
nStuff.Glitz.AnimationManager._fps = 100;
nStuff.Glitz.AnimationManager._activeAnimations = null;
nStuff.Glitz.AnimationManager._timerCookie = 0;

// ---- Do not remove this footer ----
// Generated using Script# v0.3.0.0 (http://projects.nikhilk.net)
// -----------------------------------
