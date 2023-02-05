# Unity package tester

GitHub Action to test Unity packages. It wraps [Unity test runner](https://github.com/game-ci/unity-test-runner) and introduces an empty Unity project in which the target package is imported and tested.

[Activation](https://game.ci/docs/github/activation) steps must be followed before using this action.

All [inputs from Unity test runner](https://game.ci/docs/github/test-runner) are supported except for `projectPath`. The input `unityVersion` is mandatory because, contrary to Unity projects, packages are not tied to a specific Unity version.

An extra input called `scopedRegistries` allows to specify scoped registries to use during package import. Enter the value as a comma separated list of scoped registries in JSON, as shown below.

```yaml
scopedRegistries: |
  {
    "name": "General",
    "url": "https://example.com/registry",
    "scopes": [
      "com.example", "com.example.tools.physics"
    ]
  },
  {
    "name": "Tools",
    "url": "https://mycompany.example.com/tools-registry",
    "scopes": [
      "com.example.mycompany.tools"
    ]
  }
```

This Action uses [actions/checkout](https://github.com/actions/checkout) internally, so there's no need use it again. It can be used together with [actions/cache](https://github.com/actions/cache).

The following is an example of usage. It runs tests on 3 different versions of Unity.

```yaml
name: Test package
on: [push, pull_request]

jobs:
  test-package:
    runs-on: ubuntu-latest
    strategy:
      matrix:
        version: [2020.3.44f1, 2021.3.17f1, 2022.2.5f1]
    steps:
      - uses: actions/cache@v3
        with:
          path: Library
          key: Library-${{ matrix.version }}
          restore-keys: |
            Library-
      - uses: willykc/unity-package-tester@v1
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
        with:
          unityVersion: ${{ matrix.version }}
          artifactsPath: 'artifacts/${{ matrix.version }}'
          githubToken: ${{ secrets.GITHUB_TOKEN }}
```