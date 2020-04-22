
function SetAjaxCursorChange() {

  $(document).ajaxStart(function () {
    $('body').css('cursor', 'wait');
  });

  $(document).ajaxStop(function () {
    $('body').css('cursor', 'auto');
  });

}


function promptToClipboard(ctl) {
  window.prompt("Copia in clipboard: Ctrl+C, Invio", $(ctl).text());
}

function selectText(containerid) {
  var range;

  if (document.selection) {
    range = document.body.createTextRange();
    range.moveToElementText(document.getElementById(containerid));
    range.select();
  } else if (window.getSelection()) {
    range = document.createRange();
    range.selectNode(document.getElementById(containerid));
    window.getSelection().removeAllRanges();
    window.getSelection().addRange(range);
  }

  //if (window.clipboardData) {
  //  window.clipboardData.setData('Text', range.toString())
  //}

}

var ajaxQueue = $({});

$.ajaxQueue = function (ajaxOpts) {

  // Hold the original complete function
  var oldComplete = ajaxOpts.complete;

  // Queue our ajax request
  ajaxQueue.queue(function (next) {

    // Create a complete callback to invoke the next event in the queue
    ajaxOpts.complete = function () {

      // Invoke the original complete if it was there
      if (oldComplete) {
        oldComplete.apply(this, arguments);
      }

      // Run the next query in the queue
      next();
    };

    // Run the query
    $.ajax(ajaxOpts);
  });
};

//fast template variabile subst {placeHolder} 
function parseTemplate(tmpl, data) {
  var regexp;

  for (placeholder in data) {
    tmpl = tmpl.replace(new RegExp('{' + placeholder + '}', 'g'), data[placeholder]);
  }
  return tmpl;
}