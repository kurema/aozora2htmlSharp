# aozora2htmlSharp
[aozora2html](https://github.com/aozorahack/aozora2html)のC#版です。

## 実行
コマンドは`aozora2html`です。以下のように実行します。

```shell-session
$ aozora2html foo.txt foo.html
```

こうすると、青空文庫記法で書かれた`foo.txt`を`foo.html`に変換します。

また、青空文庫サイトで配布している、中にテキストファイルが同梱されているzip形式のファイルも変換できます。

```shell-session
$ aozora2html foo.zip foo.html
```

第1引数にURLを指定すると、そのURLのファイルをダウンロードして変換します。

```shell-session
$ aozora2html http://example.jp/foo/bar.zip foo.html
```

第2引数を省略すると、ファイルではなく標準出力に変換結果を出力します。

```shell-session
$ aozora2html foo.txt
```

コマンドラインオプションとして`--gaiji-dir`と`--css-files`、`--use-jisx0213`、`--use-unicode`があります。

* `--gaiji-dir`は外字画像のパスを指定します。
* `--css-files`はCSSファイルを`,`区切りで指定します。
* `--use-jisx0213`はJIS X 0213の外字画像を使わず、数値実体参照として表示します。
* `--use-unicode`はUnicodeのコードポイントが指定されている外字を数値実体参照として表示します。

可能な限り数値実体参照を使って表示するには、以下のようにオプションを指定します。

```shell-session
$ aozora2html --use-jisx0213 --use-unicode foo.txt
```

## 関連
| レポジトリ | 概要 |
| -- | -- |
| [AozoraGaijiChukiXml](https://github.com/kurema/AozoraGaijiChukiXml) | [青空文庫・外字注記辞書【第八版】](https://www.aozora.gr.jp/gaiji_chuki/)のXML形式 |

## License
MIT
