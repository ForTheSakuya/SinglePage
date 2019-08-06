import React from "react";
import '../assets/style/upload.less'
import { Upload, Icon, message } from 'antd';
import { http } from "src/utils/http";
// import { http } from "src/utils/http";

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
            // action: 
            // (file: RcFile) => {
            //     http.post('https://www.mocky.io/v2/5cc8019d300000980a055e76', file).then(res => {
            //         console.info(res)
            //     })
            //     return '';
            // },
            customRequest: (task: object) => {
                this.init(task);
            },
            showUploadList: false,
            fileList: this.state.fileList,
            onChange: (info: any) => {
                const { status } = info.file;
                this.onFileListChange(info.fileList);
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
        // openFileDialogOnClick={false}
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

        </div>
    }

    private init(option: any) {
        const task = {
            file: option.file,
            sizeTotal: option.file.size,
            chunkTotal: Math.ceil(option.file.size / this.chunkSize),
            chunkSize: this.chunkSize,
            chunkIndex: 0,
            status: 'init'
        }
        console.info(option, task)
        this.upload(task)
    }

    private upload(task: any) {
        http.post('', task).then(res => {
            console.log(res)
            if (res.data) {
                task.status = res.data.status
                if (task.chunkIndex === task.chunkTotal) {
                    task.status = 'done'
                } else {
                    task.status = 'uploading'
                    task.chunkIndex++
                    this.upload(task)
                }
            } else {
                task.status = 'error'
            }
        });
    }
}

export default WebUpload