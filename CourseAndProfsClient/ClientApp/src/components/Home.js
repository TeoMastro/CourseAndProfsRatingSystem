import React, { Component } from 'react';
import { DataGrid, GridColumn, Form, Dialog, TextBox, NumberBox, Label, LinkButton, ButtonGroup } from 'rc-easyui';


export class Home extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            data: this.getData(),
            editingRow: null,
            model: {},
            rules: {
                'itemid': 'required',
                'name': ['required', 'length[5,10]']
            },
            errors: {},
            title: '',
            closed: true
        }
    }
    getData() {
        return [
            { "code": "FI-SW-01", "name": "Koi", "unitcost": 10.00, "status": "P", "listprice": 36.50, "attr": "Large", "itemid": "EST-1" },
            { "code": "K9-DL-01", "name": "Dalmation", "unitcost": 12.00, "status": "P", "listprice": 18.50, "attr": "Spotted Adult Female", "itemid": "EST-10" },
            { "code": "RP-SN-01", "name": "Rattlesnake", "unitcost": 12.00, "status": "P", "listprice": 38.50, "attr": "Venomless", "itemid": "EST-11" },
            { "code": "RP-SN-01", "name": "Rattlesnake", "unitcost": 12.00, "status": "P", "listprice": 26.50, "attr": "Rattleless", "itemid": "EST-12" },
            { "code": "RP-LI-02", "name": "Iguana", "unitcost": 12.00, "status": "P", "listprice": 35.50, "attr": "Green Adult", "itemid": "EST-13" },
            { "code": "FL-DSH-01", "name": "Manx", "unitcost": 12.00, "status": "P", "listprice": 158.50, "attr": "Tailless", "itemid": "EST-14" },
            { "code": "FL-DSH-01", "name": "Manx", "unitcost": 12.00, "status": "P", "listprice": 83.50, "attr": "With tail", "itemid": "EST-15" },
            { "code": "FL-DLH-02", "name": "Persian", "unitcost": 12.00, "status": "P", "listprice": 23.50, "attr": "Adult Female", "itemid": "EST-16" },
            { "code": "FL-DLH-02", "name": "Persian", "unitcost": 12.00, "status": "P", "listprice": 89.50, "attr": "Adult Male", "itemid": "EST-17" },
            { "code": "AV-CB-01", "name": "Amazon Parrot", "unitcost": 92.00, "status": "P", "listprice": 63.50, "attr": "Adult Male", "itemid": "EST-18" }
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
    editRow(row) {
        this.setState({
            editingRow: row,
            model: Object.assign({}, row),
            title: 'Edit',
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
                        <div style={{ marginBottom: 10 }}>
                            <Label htmlFor="itemid">Item ID:</Label>
                            <TextBox inputId="itemid" name="itemid" value={row.itemid} style={{ width: 220 }}></TextBox>
                            <div className="error">{this.getError('itemid')}</div>
                        </div>
                        <div style={{ marginBottom: 10 }}>
                            <Label htmlFor="name">Name:</Label>
                            <TextBox inputId="name" name="name" value={row.name} style={{ width: 220 }}></TextBox>
                            <div className="error">{this.getError('name')}</div>
                        </div>
                        <div style={{ marginBottom: 10 }}>
                            <Label htmlFor="listprice">List Price:</Label>
                            <NumberBox inputId="listprice" name="listprice" value={row.listprice} precision={1} style={{ width: 220 }}></NumberBox>
                        </div>
                        <div style={{ marginBottom: 10 }}>
                            <Label htmlFor="unitcost">Unit Cost:</Label>
                            <NumberBox inputId="unitcost" name="unitcost" value={row.unitcost} style={{ width: 220 }}></NumberBox>
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
        return (
            <div>
                <h2>Dialog Editing</h2>
                <DataGrid data={this.state.data} style={{ height: 750 }}>
                    <GridColumn field="itemid" title="Item ID"></GridColumn>
                    <GridColumn field="name" title="Name"></GridColumn>
                    <GridColumn field="listprice" title="List Price" align="right"></GridColumn>
                    <GridColumn field="unitcost" title="Unit Cost" align="right"></GridColumn>
                    <GridColumn field="status" title="Status" align="center"></GridColumn>
                    <GridColumn field="act" title="Actions" align="center" width={110}
                        render={({ row }) => (
                            <div>
                                <ButtonGroup>
                                    <LinkButton onClick={() => this.editRow(row)}>Edit</LinkButton>
                                    <LinkButton onClick={() => this.deleteRow(row)}>Delete</LinkButton>
                                </ButtonGroup>
                            </div>
                        )}
                    />
                </DataGrid>
                {this.renderDialog()}
            </div>
        );
    }
}

export default Home;
