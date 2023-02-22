# CubismPortfolioPlugin

## 概要

本プロジェクトは Cubism SDK ワークショップ #4にて作成・解説を行ったプロジェクトです。複数体のモデルの表示、カーソルへの視線追従、パーツ単位での乗算色・スクリーン色の操作、アニメーション再生などの機能を備えています。

第四回 Cubism SDK ワークショップについては以下の記事をご確認ください。

[第四回 Cubism SDK ワークショップ講演レポート](https://note.com/live2dnote/n/n0fe762b84f53)

利用には事前に下記のダウンロードが必要です。

* Unity Hub
* Unity（推奨:2021 LTS、開発:2021.3.8f1）
* SDK for Unity（開発: Cubism 4 SDK for Unity R5）

## Cubism SDK for Unity

モデルを表示、操作するための各種機能を提供します。

本プロジェクトには Live2D Cubism for Unity は含まれておりません。

Live2D 公式ホームページの Cubism SDK ダウンロードページよりダウンロードし、プロジェクトへ事前にインポートの上で本プロジェクトをご利用ください。

[Cubism SDK](https://www.live2d.com/download/cubism-sdk/)


## SDKマニュアル・チュートリアル

[Cubism SDK Manual](https://docs.live2d.com/cubism-sdk-manual/top/)

[Cubism SDK Tutorial](https://docs.live2d.com/cubism-sdk-tutorials/top/)


## ディレクトリ構成

本プロジェクトのディレクトリの構成

```
Assets/Live2D/Cubism
└─ PortfolioPlugin      # PortfolioPlugin が含まれるディレクトリ
    ├─ Editor           # エディタ拡張が含まれるディレクトリ
    └─ CompletePackage  # 完成したシーンが含まれるディレクトリ
```


## プロジェクトの利用方法

### モデルの導入方法

1. モデルの組み込みファイルの入ったフォルダをUnityのプロジェクトウィンドウへドラッグアンドドロップする
2. `Assets/Live2D/Cubism/PortfolioPlugin/CompletePackage` 以下のシーン `Sample.unity` を開く
3. 1.で生成されたプレハブをシーンのヒエラルキーウィンドウへドラッグアンドドロップする


## Cubism SDK を利用したアプリケーション開発について

`Cubism SDK for Unity` を利用してアプリケーションをリリースする場合には出版許諾契約等の各種契約が必要となります。

詳しくは下記リンク先をご確認ください。

[Cubism SDK リリースライセンス](https://www.live2d.com/download/cubism-sdk/release-license/)
