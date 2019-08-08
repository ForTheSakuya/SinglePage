import React from "react";
import '../assets/style/upload.less'
import { Upload, Icon, message, Progress } from 'antd';
import { http } from "src/utils/http";

const { Dragger } = Upload;
interface IWebUploadState {
    fileList: any[]
}

class WebUpload extends React.Component<any, IWebUploadState> {
    private chunkSize = 20 * 1024 * 1024;
    constructor(state: IWebUploadState) {
        super(state)
        this.state = { fileList: [] }
    }

    public onFileListChange(fileList: any[]) {
        this.setState({ fileList: [...fileList] });
    }

    public render() {
        const props = {
            multiple: true,
            customRequest: (task: object) => {
                this.init(task);
            },
            showUploadList: false,
            onChange: (info: any) => {
                const { status } = info.file;
                if (status !== 'uploading') {
                    console.log(info.file, info.fileList);
                    console.log(this.state.fileList);
                }
                if (status === 'done') {
                    message.success(`${info.file.name} 文件上传成功。`);// file uploaded successfully.
                } else if (status === 'error') {
                    message.error(`${info.file.name} 文件上传失败。`);// file upload failed.
                }
            },
        };

        return <div className='web-upload'>
            <Dragger {...props} >
                <div className={'btns' + (this.state.fileList.length > 0 ? ' hadList' : '')}>
                    <p className="ant-upload-drag-icon">
                        <Icon type="inbox" />
                        上传文件
                        </p>
                    <p>点击或拖拽文件到此区域上传</p>
                </div>
            </Dragger>
            <div className={'file-list' + (this.state.fileList.length > 0 ? ' active' : '')}>
                {
                    this.state.fileList.map((item, index) => {
                        return <div key={index} className='file-item'>
                            <div className='item-info'>
                                <span className='title'>{index + 1}. {item.file.name}</span>
                                <div className='btn' onClick={this.deleteTask.bind(this, index)}><i>×</i></div>
                            </div>
                            <Progress percent={item.percent} status={this.getProgress(item)} />
                        </div>
                    })
                }
            </div>
        </div>
    }

    private init(option: any) {
        const { file } = option
        const task = {
            file,
            sizeTotal: file.size,
            chunkTotal: Math.ceil(file.size / this.chunkSize),
            chunkIndex: 0,
            chunkSize: this.chunkSize,
            status: 'init',
            type: this.getEntityType(file),
            uid: ''
        }
        this.state.fileList.push(task)
        this.onFileListChange(this.state.fileList);
        console.info(option, task)
        this.upload(task)
    }

    private upload = (task: any) => {
        const form = new FormData()
        const { file } = task
        const start = task.chunkIndex * task.chunkSize
        const end = Math.min(file.size, start + task.chunkSize)
        const data = file.slice(start, end);
        if (data === null || data.size === 0) {
            task.status = 'error';
            task.file = null;
            return;
        }

        form.append('fileData', data);
        if (task.uid) {
            form.append('fileId', task.uid);
        }
        form.append('filePath', task.filePath || '');
        form.append('fileName', file.name);
        form.append('chunkIndex', task.chunkIndex);
        form.append('chunkTotal', task.chunkTotal);
        form.append('chunkSize', task.chunkSize + '');
        form.append('status', task.status);
        form.append('size', task.sizeTotal);
        form.append('entityType', task.type);
        form.append('md5', '');

        http.post('/api/upload/multipart', form).then(res => {
            console.log(res)
            if (res.data) {
                task.status = res.data.status
                if (res.data.status === 'error') {
                    return;
                }
                task.chunkIndex++
                task.uid = res.data.fileId
                task.filePath = res.data.filePath
                if (task.chunkIndex === task.chunkTotal) {
                    task.status = 'done'
                    message.success(`${file.name} 文件上传成功。`);// file uploaded successfully.
                } else {
                    task.status = 'uploading'
                    this.upload(task)
                }
            } else {
                task.status = 'error'
            }
            task.percent = Math.floor((task.chunkIndex / task.chunkTotal) * 100)
            this.onFileListChange(this.state.fileList);
        }).catch(() => {
            task.status = 'error'
            this.onFileListChange(this.state.fileList);
        });
    }

    private deleteTask = (index: number) => {
        this.state.fileList.splice(index, 1)
        this.onFileListChange(this.state.fileList)
    }

    private getProgress = (item: any) => {
        if (item.status === 'uploading') {
            return 'active'
        } else if (item.status === 'error') {
            return 'exception'
        }
        return
    }

    private getEntityType(file: any) {
        if (file.type.indexOf('video') > -1) {
            return 'Clip'
        } else if (file.type.indexOf('image') > -1) {
            return 'Picture'
        } else if (file.type.indexOf('audio') > -1) {
            return 'Audio'
        } else if (file.type.indexOf('text') > -1) {
            return 'Document'
        } else {
            return 'Other'
        }
    }
}

export default WebUpload