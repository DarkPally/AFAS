# 取证规则编辑器说明文档

## 概述与工具使用流程

目前，每个规则是一个互相独立的json格式文本，按照一定规则填写对应的项目，取证工具便能正确地解析该规则，并对设定的工作目录进行单应用的取证分析。

当前版本功能还比较简陋，接下来会进行大更新，目前版本正确使用流程如下：

1. 在 **规则编辑** 页面下，点击 **加载规则** 按钮，工具会自动读取RuleLibrary目录下的有效规则
2. 在 **规则包名** 右侧的下拉列表中选择待编辑的规则，之后下方的文本框会显示规则的文本内容。
3. 对规则进行编辑，完成后点击 **测试当前规则** 按钮
   1. 如果结果返回 “测试成功！”，则说明规则没有语法上的问题；反之，报错则说明json格式语法或类型上存在错误，鼠标悬停在 状态文字 上方会显示具体的异常信息。
   2. 如果需要运行测试，即使没有更改规则文本，也需要点击此按钮；只有点击此按钮，工具才会把工作的规则切换为当前规则。
4. 测试完成后，在上方正常配置工作路径，点击 **文件夹解析** 即可进行单例测试。
5. 无论何时，只要没有点击 **保存当前规则** 按钮，规则都不会保存，所以要及时点击。
6. 目前尚未支持新建规则，新增规则时可直接在RuleLibrary目录下复制一个txt文档并重命名。



## 规则语法说明

规则为json格式文本，下面将介绍每个节点的属性。

需要注意的是，并不是每个属性都要写在规则上，如果不明文写出某个属性，那么该属性就会以默认值出现。默认值包括（0、false、null）等。

### 根节点

根节点的属性固定，属性名称和作用如下表所示：

属性名称 | 说明与作用
------------ | -------------
Name | 规则包的正规名称，应准确填写包名。为做区分IOS的规则后跟(IOS)
Version | 版本信息，目前没用，用于后续做版本区分
Desc | 规则包的描述名称，显示在选择列表和取证结果中的名称
Items | 规则条目的集合，这是一个json数组类型，里面按序记载了一系列具体的规则，详见 **规则节点说明** 条目
Scripts | 脚本集合，同样是json数组。考虑到有些脚本要经常复用，因此可把需要复用的脚本放在此处管理，详见 **数据处理规则** 条目
RootPathPrepares | 用特征文件匹配法获取根路径的集合，直接用路径/正则路径法可无视此属性，详见 **文件捕获规则** 条目

### 规则节点说明

目前有六种类型的规则，由"$type"属性控制具体的规则类型，使用时只需作为枚举量粘贴到相应位置即可，列表如下：

规则类型 | 对应的$type属性的值
------------ | -------------
文件捕获 | "AFAS.Library.FileCatchInfo, AFAS.Library"
文件处理 | "AFAS.Library.FileProcessInfo, AFAS.Library"
数据捕获 | "AFAS.Library.DataCatchInfo, AFAS.Library"
数据处理 | "AFAS.Library.DataProcessInfo, AFAS.Library"
数据关联 | "AFAS.Library.DataAssociateInfo, AFAS.Library"
结果标记 | "AFAS.Library.DataMarkInfo, AFAS.Library"

#### 文件捕获规则

文件捕获规则的作用为，根据本规则的描述捕获某一个，或某一类文件，并将捕获的文件信息存放到数据表中。本规则涉及到的属性如下：

属性名称 | 作用说明
------------ | -------------
Key | 输出数据表的索引键，在后续操作中可使用该Key访问到数据表
RootPathType| 根目录类型，填写“Default”或不填时以RootPath为固定路径；填“PathPrepare”时，RootPath由特征文件匹配获得。
RootPath | 在直接使用路径/正则匹配时，目标文件的根目录。可不填该项，默认值为data/data/[包名]/；在特征文件匹配时，RootPath为根节点下RootPathPrepares内对应的索引
RelativePath | 目标文件的相对路径
IsRegEx | 目标文件的相对路径是否包含正则

根节点下的RootPathPrepares内可包含一个或多个元素，如：

```json
 "RootPathPrepares":[
   {
       "Name":"root",
       "PathRegexes":
       [
           "_store_(.*)/messenger_contacts.v1/fbomnistore.db$",
       ]
   }
```
PathRegexes下存放一个或多个正则，只要某目录下的所有正则都能匹配，就认为是一个有效的根目录，并且以Name作为索引。

#### 文件处理规则

如果捕获到的文件是经过特殊编码、加密的，可在此步进行处理，目前暂时用不到，略去。

#### 数据捕获规则

在文件已经捕获、处理后，本规则的作用是指导工具如何从文件提取数据，属性列表如下：

属性名称 | 作用说明
------------ | -------------
Key | 输入索引，一般为文件捕获规则中的Key值，作为源文件的索引
TableKey | 输出索引，即数据捕获完毕后，输出的数据表的索引
DataPath | 数据路径，里面存储的内容由Type属性指定
Type| 捕获类型，目前有五种类型：<br>“Binary”:二进制处理法，DataPath存储脚本文本<br>“Text”:文本文件处理法，DataPath存储可提取文本的正则<br>“Xml”:处理Xml，将xml转化为数据库格式，DataPath为转化后的表名<br>“Database”：处理数据库，DataPath为数据表名<br>“DatabaseWithRegEx”：处理数据库，但DataPath为数据表名的正则
Select | 如果需要对捕获到的结果进行筛选，可填写此属性，填写方法参考DataTable.Select()函数的语法。
CatchToFileDataTree| bool值，默认为false,true时将把本表结果作为文件表的子表，属于数据关联的内容。

#### 数据处理规则

捕获到的原始数据可能有冗余信息，或者经过加密，因此可在此规则中指导工具对数据进行处理。属性列表如下：

属性名称 | 作用说明
------------ | -------------
Key | 输入索引，即为源数据表的索引，也可以是文件表
ColumnName | 待处理的列名，即一条规则只能处理一列
OutputColumnName| 处理后的列名，不可与原表内的名称冲突。不填即默认为 [ColumnName]+"_proc"
Content | 处理方法的内容，由Type指定
Type| 处理方法，"RegEx": 使用Content内的正则进行裁剪<br>"Script": 使用Content脚本进行处理<br>"ScriptName": Content存放的是脚本索引


根节点下的Scripts内可包含一个或多个元素，如：
```json
"Scripts":[
{
         "Name":"UnixTimeStamp",
         "Content":"
                local startTime =  clr.System.DateTime(1970, 1, 1);
                startTime =clr.System.TimeZone.CurrentTimeZone.ToLocalTime(startTime);
                return startTime:AddSeconds(data);",
}
]
```
Content下存放Lua脚本，Name为该脚本的索引。

脚本环境内置了一些动态环境变量，用于获取当前的运行状态：

变量名称| 说明
------------ | -------------
data | 当前待处理的原始数据
dataTableRow | 当前正在处理的数据所属的行
dataTable | 当前正在处理的数据表
parentFile | 当前正在处理的数据表的所属文件表
forensicPackage | 当前的取证环境

Lua脚本的编写方法几乎可等价于C#代码，详情可参考:<br>https://www.darkstar.cn/archives/1586<br>
https://github.com/neolithos/neolua


#### 数据关联规则

所谓数据关联，是指建立数据表与数据表之间的父子关系。

例如，现有好友列表集合Friends，和好友消息列表集合Msgs，那么可以用本规则实现以下操作：
1. 把Friends表作为父表，按行拆解构成多张新表Friend_X。每个Friend_X表代表单独的一个好友的信息，并且成为Friends的子表；
2. 依据Friend_X中的Friend ID将好友X的记录单独从Msgs筛选出来，作为一张新表,并作为Friend_X的子表。

当然也可以反过来，把Msgs作为父表，把同属于一个friend的msg筛选出来后，把friend作为子表。

利用下面的属性即可指导工具进行关联操作：

属性名称| 作用
------------ | -------------
Key | 筛选后的表的集合，目前不常用，可空
ParentTableKey | 父表的表索引名称
ParentTableColumn | 父表中，需要匹配的元素的列名
ChildTableKey | 子表的索引名称
AssociateColumn | 子表中需要匹配的元素的列名
Type | 匹配模式，目前有三种：<br>"InSameFile" : 在同文件内的表中匹配，即仅当父子两个表都在同一个数据库文件中，才进行关联<br>"AcrossFile“ :跨文件匹配，即父子两表不受文件的限制<br>ChildFileColumn :跨文件匹配，且子对象需要匹配的元素在文件信息表中，不在自己的表中

#### 结果标记规则

一般来说，所有重要的结果都要通过此规则进行标记。否则，数据将被视为中间数据保存在“数据集合”节点下，而不是最终的输出数据。此外，输出结果可以在本规则下修改描述（表名、列名），或对列增加其他有意义的自定义标签，主要属性列表如下：


属性名称| 作用
------------ | -------------
Key | 需要标记的表索引
TableDesc | 表的描述性名称
TableDescType | （目前可忽略）描述的对象，0仅根节点，1仅子节点，2全部
TableDescScript | 描述的脚本，用于动态指定表的名称，优先级高于TableDesc
NotShowAtRoot | bool类型，如果为true，那么本节点将不在结果中单独列出（一般它属于其他节点的子节点，不用单独拎出来再显示一遍）
ColumnDescs | json数组类型，每个元素含有Name、Desc、Mark三个元素，用于标记列名
OnlyShowDesc | bool类型，如果为true，那么将只显示列名中带有Desc的项，其他项不显示
