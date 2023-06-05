using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers;

public interface ITemplate
{
	string DynamicContents { get; }
	string Header { get; }
	string Footer { get; }

}

public class TemplateAozora : ITemplate
{
	public const string DYNAMIC_CONTENTS = """
		<div id="card">
		<hr />
		<br />
		<a href="JavaScript:goLibCard();" id="goAZLibCard">●図書カード</a><script type="text/javascript" src="../../contents.js"></script>
		<script type="text/javascript" src="../../golibcard.js"></script>
		</div>
		""";
	public const string HEADER = """
		<?xml version="1.0" encoding="Shift_JIS"?>
		<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN"
		    "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
		<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="ja" >
		<head>
			<meta http-equiv="Content-Type" content="text/html;charset=Shift_JIS" />
			<meta http-equiv="content-style-type" content="text/css" />

		""";

	public const string FOOTER = "</body>\r\n</html>\r\n";

	public string DynamicContents => DYNAMIC_CONTENTS;

	public string Header => HEADER;

	public string Footer => FOOTER;
}

public class TemplateBasic : ITemplate
{
	public const string HEADER = """
		<!DOCTYPE html>
		<html lang="ja">
		<head>

		""";

	public string DynamicContents => string.Empty;

	public string Header => HEADER;

	public string Footer => TemplateAozora.FOOTER;
}

public class TemplateGeneral : ITemplate
{
	public string DynamicContents { get; set; } = string.Empty;

	public string Header { get; set; } = string.Empty;

	public string Footer { get; set; } = string.Empty;
}
