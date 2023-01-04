# aozora2htmlSharp
[aozorahack/aozora2html](https://github.com/aozorahack/aozora2html)のC#版です。

## v1.0
移植元v3.0.0準拠の実装をv1.0系統にしています。
挙動だけでなくメソッド名やテストもほぼ同じです。

v2.0以降はその限りではありません。

* [ドキュメント](docs/v1.0/readme.md)
* [branch](https://github.com/kurema/aozora2htmlSharp/tree/v1.0)
* [Release](https://github.com/kurema/aozora2htmlSharp/releases/tag/v1.0.0)

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

コマンドラインオプションとして`--gaiji-dir`と`--css-files`、`--use-jisx0213`、`--use-unicode`、`--check-newline`があります。

* `--gaiji-dir`は外字画像のパスを指定します。
* `--css-files`はCSSファイルを`,`区切りで指定します。
* `--use-jisx0213`はJIS X 0213の外字画像を使わず、数値実体参照として表示します。
* `--use-unicode`はUnicodeのコードポイントが指定されている外字を数値実体参照として表示します。
* `--check-newline false`は改行コードのチェックを省略します。

可能な限り数値実体参照を使って表示するには、以下のようにオプションを指定します。

```shell-session
$ aozora2html --use-jisx0213 --use-unicode foo.txt
```

## 関連
| レポジトリ | 概要 |
| -- | -- |
| [aozorahack/aozora2html](https://github.com/aozorahack/aozora2html) | Rubyで実装されたオリジナル。 |
| [kurema/AozoraGaijiChukiXml](https://github.com/kurema/AozoraGaijiChukiXml) | [青空文庫・外字注記辞書【第八版】](https://www.aozora.gr.jp/gaiji_chuki/)のXML形式 |

## License
MIT
