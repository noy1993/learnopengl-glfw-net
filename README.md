## 使用 glfw 学习 opengl，课程参考 https://learnopengl-cn.github.io/

为了便于参考 C++ 学习 glfw 该示例使用的是 silk.net 的 glfw 的原始包装器版本，并没有使用其封装后版本，实际项目视情况而定。

opengl 默认使用的是 es3.0 版本编写，是为了兼容以后学习 webgl 2.0

因为 System.Drawing.Common 在 .net6 后只能在 Windows 平台上运行，为了兼容多个平台的学习，将图片相关绘制操作使用 SkiaSharp 库代替
