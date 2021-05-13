import React, { Component } from 'react';
import { DataGrid, GridColumn, Form, Dialog, TextBox, NumberBox, Label, LinkButton, ComboBox,ButtonGroup } from 'rc-easyui';


export class Home extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            data: this.getData(),
            editingRow: null,
            model: {},
            rules: {
                'itemid': 'required',
                'review': 'required'
            },
            errors: {},
            title: '',
            closed: true
        }
    }
    getData() {
        return [
            {
                "id": 1,
                "fullName": "Iliou Chi",
                "mail": "mail@iliou.il",
                "phone": "231233232",
                "office": "e12",
                "eOffice": "zooom",
                "department": "Informatics",
                "averageRating": 10
            },
            {
              "id": 2,
              "fullName": "Asdre",
              "mail": "mail@azdre.az",
              "phone": "2312323312",
              "office": "e13",
              "eOffice": "zoomAz123",
              "department": "Informatics",
              "averageRating": 0
            }

        ]
    }
    getError(name) {
        const { errors } = this.state;
        if (!errors) {
            return null;
        }
        return errors[name] && errors[name].length
            ? errors[name][0]
            : null;
    }
    addReview(row) {
        this.setState({
            model: Object.assign({}),
            title: 'Add',
            closed: false
        });
    }
    saveRow() {
        this.form.validate(() => {
            if (this.form.valid()) {
                let row = Object.assign({}, this.state.editingRow, this.state.model);
                let data = this.state.data.slice();
                let index = data.indexOf(this.state.editingRow);
                data.splice(index, 1, row);
                this.setState({
                    data: data,
                    closed: true
                })
            }
        })
    }
    clearValue() {
        this.setState({ value: null })
    }
    deleteRow(row) {
        this.setState({
            data: this.state.data.filter(r => r !== row)
        })
    }
    renderDialog() {
        const row = this.state.model;
        const { title, closed, rules } = this.state;
        return (
            <Dialog modal title={title} closed={closed} onClose={() => this.setState({ closed: true })}>
                <div className="f-full" style={{ padding: '20px 50px' }}>
                    <Form className="f-full"
                        ref={ref => this.form = ref}
                        model={row}
                        rules={rules}
                        onValidate={(errors) => this.setState({ errors: errors })}
                    >
                        <div>
                            <Label htmlFor="cProf" align="top">Select a Professor:</Label>
                            <ComboBox
                                inputId="cProf"
                                iconCls="icon-man"
                                editable={false}
                                data={this.state.data}
                                value={this.state.value}
                                style={{ width: '100%' }}
                                onChange={(value) => this.setState({ value: value })}
                                addonRight={() => (
                                    <span className="textbox-icon icon-clear" title="Clear value" onClick={this.state.value= null }></span>
                                )}
                            />
                            <p>You selected: {this.state.value}</p>
                        </div>
                        <div>
                            <Label htmlFor="cCourse" align="top">Select a Course:</Label>
                            <ComboBox
                                inputId="cCourse"
                                iconCls="icon-man"
                                editable={false}
                                data={this.state.data}
                                value={this.state.value}
                                style={{ width: '100%' }}
                                onChange={(value) => this.setState({ value: value })}
                                addonRight={() => (
                                    <span className="textbox-icon icon-clear" title="Clear value" onClick={this.state.value = null}></span>
                                )}
                            />
                            <p>You selected: {this.state.value}</p>
                        </div>

                        <div style={{ marginBottom: 10 }}>
                            <Label htmlFor="tscore" style={{ width: 250 }}>Τι βαθμο πηρατε στο μαθημα;</Label>
                            <TextBox inputId="tscore" name="tscore" value={row.name} style={{ width: 50 }}></TextBox>
                        </div>

                        <div style={{ marginBottom: 10 }}>
                            <Label htmlFor="treview" style={{ width: 250 }}>Τι βαθμο βαζετε στον καθηγητη;</Label>
                            <TextBox inputId="treview" name="treview" value={row.name} style={{ width: 50 }}></TextBox>
                            <div className="error">{this.getError('review')}</div>
                        </div>

                        <div style={{ marginBottom: 10 }}>
                            <Label htmlFor="tcomments" align="top">Σχολια:</Label>
                            <TextBox inputId="tcomments" multiline name="tcomments" value={row.name} style={{ width: '100%', height: 120  }}></TextBox>
                        </div>

                    </Form>
                </div>
                <div className="dialog-button">
                    <LinkButton style={{ width: 80 }} onClick={() => this.saveRow()}>Save</LinkButton>
                    <LinkButton style={{ width: 80 }} onClick={() => this.setState({ closed: true })}>Close</LinkButton>
                </div>
            </Dialog>
        )
    }
    render() {
        const clearValue = () => {
            this.setState({ value: null })
        }
        return (
            <div>
                <h2>Professor's ratings</h2>
                <LinkButton style={{ width: '100%' }} onClick={() => this.addReview()}>Add your review</LinkButton>
                <DataGrid data={this.state.data} style={{ height: 550, padding:'15' }}>


                    <GridColumn field="id" title="PrID" hidden="true"></GridColumn>
                    <GridColumn field="fullName" title="Name" align="center"></GridColumn>
                    <GridColumn field="mail" title="Mail" align="center"></GridColumn>
                    <GridColumn field="department" title="Department" align="center"></GridColumn>
                    <GridColumn field="averageRating" title="Average rating" align="center"></GridColumn>
                </DataGrid>
                {this.renderDialog()}
            </div>
        );
    }
}

export default Home;
