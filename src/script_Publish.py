import os
import sys
from pathlib import Path

import my
from my import (my_call, print_info, print_error, remove_tree, remove_file,
                make_dir, copy_tree, copy_and_merge_tree, copy_file,
                pushd, create_zip_file, move_file, wait_enter_key)


def main():
    # 切换到脚本所在目录
    my.ch_dir_to_entry_script_dir()

    # 读取环境变量 msbuild
    msbuild = os.environ.get('msbuild', '')
    if not msbuild:
        print_error("系统环境变量 msbuild 必须设置到 MSBuild.exe 的路径")
        sys.exit(1)

    temp_dir = r"D:\temp\Log2Window"
    bin_dir = os.path.join(temp_dir, "bin")
    example_dir = os.path.join(temp_dir, "ExampleProject")
    zip_file = r"D:\temp\Log2Window.zip"

    # 清空临时目录
    print_info("清理临时目录...")
    remove_tree(temp_dir)
    remove_file(zip_file)

    # 创建输出目录
    make_dir(temp_dir)
    make_dir(example_dir)

    # 编译
    print_info("编译项目...")
    my_call([
        msbuild,
        "/t:rebuild",
        r"/property:OutputPath=d:\temp\Log2Window",
        "/property:Configuration=Release",
        "/property:DeployOnBuild=true",
        r"Log2Window\Log2Window.csproj"
    ], shell=False)

    # 复制示例项目
    print_info("复制示例项目...")
    copy_tree("TestLog4net", os.path.join(example_dir, "TestLog4net"))
    copy_tree("TestNLog", os.path.join(example_dir, "TestNLog"))

    # 清理示例项目中的 bin/obj/packages
    print_info("清理示例项目的构建产物...")
    for name in ['bin', 'obj', 'packages']:
        for p in Path(example_dir).rglob(name):
            if p.is_dir():
                remove_tree(str(p))

    # 创建 bin 目录并移动输出文件
    print_info("整理输出文件...")
    make_dir(bin_dir)

    for p in Path(temp_dir).glob("*.dll"):
        move_file(str(p), bin_dir)
    for p in Path(temp_dir).glob("*.xml"):
        move_file(str(p), bin_dir)

    # 压缩
    print_info("创建压缩包...")
    with pushd(r"D:\temp"):
        create_zip_file("Log2Window")

    # 复制到云盘
    print_info("复制到云盘...")
    copy_and_merge_tree(temp_dir, r"D:\Users\Alan\Documents\YunPan\Program Files\Log2Window")
    copy_file(zip_file, r"D:\Users\Alan\Documents\YunPan\Program Files")

    print_info("全部完成.")


if __name__ == "__main__":
    my.call_main(main)
