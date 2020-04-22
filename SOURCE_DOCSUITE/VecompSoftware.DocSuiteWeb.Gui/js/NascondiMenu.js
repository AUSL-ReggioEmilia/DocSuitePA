document.onmouseover = Riduci

function Riduci()
{
	try
	{
		top.fstMain.cols = '25,*';
		//top.fstMain.frameSpacing = '0';
		top.menu.document.getElementById("TreeView").style.display= "none";
		top.menu.document.getElementById("Menu").style.display= "inline";
	}
	catch(er)
	{
	}
}
