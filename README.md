Unity3D-Python-Editor
=====================

在unity3d里使用python
unity版本5.6.1

> NOTE
>
> 我这是用@cesardeazevedo那里弄到的,然后我精简了一下,现在只需要改动一下就可以用了.操作如下:
> 在游戏物体上 绑定 PyRun.cs 在 PyRun.cs 上 绑定 PyFile和PyFilePath 就行了

```
public class PyRun : MonoBehaviour
{
    //Python文件
    public Object pyFile;
    //Python文件路径
    public string pyFilePath="Assets/src/python/";
```

## LICENSE
[MIT](./LICENSE)
