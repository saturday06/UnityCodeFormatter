# Unity Code Formatter (仮)

Unityで使えるエディタ中立のC#コードフォーマッターを目指します

# UPMから入れる方法

## Unityのパッケージマネージャーからインストールする場合

パッケージマネージャーの `Add package from git URL ...` から `https://github.com/saturday06/UnityCodeFormatter.git` を登録してください。

## `Packages/manifest.json` を直接編集する場合

dependenciesの中に次の行を追加してください。

```diff
  {
    "dependencies": {
      ...
+     "jp.leafytree.unitycodeformatter": "https://github.com/saturday06/UnityCodeFormatter.git",      
```
