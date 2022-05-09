# Binary.zip
## 対応環境
Binary.zipには以下の環境向けのバイナリが含まれています。
最適な環境は[こちら](https://docs.microsoft.com/ja-jp/dotnet/core/rid-catalog)を参照してください。

* "win-x64"
* "win-x86"
* "win-arm"
* "win-arm64"
* "linux-x64"
* "linux-musl-x64"
* "linux-arm"
* "linux-arm64"
* "rhel-x64"
* "osx-x64"
* "osx.11.0-arm64"
* "osx.10.10-x64"

## コンパイル条件
コンパイルは以下の条件で実行しています。

```
dotnet publish ${{env.ConsolePath}} --configuration Release --self-contained true -p:PublishTrimmed=true -p:PublishSingleFile=true --runtime ${item} --output publish/${item}/ -p:PublishReadyToRun=true
```

### 自己完結型
[Microsoft](https://docs.microsoft.com/ja-jp/dotnet/core/deploying/#publish-self-contained)

自己完結型でコンパイルしています。
ローカルにインストールされた共有ランタイムを使いません。
従って.NETのインストールは不要で、単なる実行ファイルとして扱えます。

一方でファイルサイズが大きくなり、脆弱性修正や機能改善が取り込まれません。

## 単一ファイル配置
[Microsoft](https://docs.microsoft.com/ja-jp/dotnet/core/deploying/single-file/overview)

単一ファイルでコンパイルしています。
実行ファイルが単一のバイナリにバンドルされます。
いくつかの非互換性がありますが、このプロジェクトとは無関係だと考えています。
テストは全ての対応環境では行っていません。

## トリミング
[Microsoft](https://docs.microsoft.com/ja-jp/dotnet/core/deploying/trimming/trim-self-contained)

トリミングをオンにしています。
バイナリサイズが小さくなります。
ただし、動的に読み込まれる場合などいくつかの大きな[非互換性](https://docs.microsoft.com/ja-jp/dotnet/core/deploying/trimming/incompatibilities)があります。
この非互換性は本プロジェクトとは無関係だと考えています。

## ReadyToRun
[Microsoft](https://docs.microsoft.com/ja-jp/dotnet/core/deploying/ready-to-run)

ReadyToRunをオンにしています。
要するにAOTで、バイナリサイズが大きくなる一方、大抵は起動時間が速くなります。
巨大なJIS X 0213辞書がソースコードに含まれているので、影響は無視できないと考えています。

# GitHub Actionsについて
v1.0.0時点のGitHub Actionsには以下の問題点があります。

* Releaseの自動作成を行わない。
  * tag作成に反応はしますが、Releaseの自動作成には対応していません。
* テストが不十分
  * sampleとの比較テストが含まれていません。
  * `dotnet publish`で作成されたバイナリでテストをしていません。