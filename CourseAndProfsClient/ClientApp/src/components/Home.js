import React, { Component, useState } from 'react';
import { DataGrid, GridColumn, Form, Dialog, TextBox, NumberBox, Label, LinkButton, ComboBox, ButtonGroup } from 'rc-easyui';
import axios from 'axios';
import { data } from 'jquery';


export class Home extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            data: this.getRatings(),
            dataProfs: this.getProfessors(),
            reviews: [],
            selectValueProf: '',
            selectGrade: '',
            selectRating: '',
            selectComments: '',
            professors: [],
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
        this.handleChangeProf = this.handleChangeProf.bind(this);
        this.handleChangeGrade = this.handleChangeGrade.bind(this);
        this.handleChangeRating = this.handleChangeRating.bind(this);
        this.handleChangeComments = this.handleChangeComments.bind(this);
    }
    handleChangeProf(e) {
        this.setState({ selectValueProf: e.target.value });
    }
    handleChangeGrade(e) {
        this.setState({ selectGrade: e.target.value });
    }
    handleChangeRating(e) {
        this.setState({ selectRating: e.target.value });
    }
    handleChangeComments(e) {
        this.setState({ selectComments: e.target.value });
    }
    getRatings() {
        axios.get(`https://${window.location.host}/AllProfessorsReviews?itemsPerPage=20&page=1`)
            .then(res => {
                const reviews = res.data.results;
                this.setState({ reviews });
            })
    } 
    getProfessors() {
        axios.get(`https://${window.location.host}/api/professor?itemsPerPage=20&page=1`)
            .then(res => {
                let idx = 0;
                const professors = [];
                res.data.results.map(obj => {
                    professors[idx] = obj.fullName;
                    idx++;
                })
                this.setState({ professors });
            })
    }
    submitReview() {
        console.log(this.state.selectValueProf); //gia kapoio logo de pairnei default value
        console.log(this.state.selectGrade); //thelei elegxo gia to an auta ta 2 states einai numbers.
        console.log(this.state.selectRating);
        console.log(this.state.selectComments);
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
        let professors = this.state.professors;
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
                            <select value={this.state.selectValueProf} onChange={this.handleChangeProf}>
                                {professors.map(professor => <option>{professor}</option>)}
                            </select>
                            <p>You selected: {this.state.selectValueProf}</p>
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
                            />
                            <p>You selected: {this.state.value}</p>
                        </div>

                        <div style={{ marginBottom: 10 }}>
                            <Label htmlFor="tscore" style={{ width: 250 }}>Τι βαθμο πηρατε στο μαθημα;</Label>
                            <input value={this.state.selectGrade} onChange={this.handleChangeGrade} style={{ width: 50 }}></input>
                        </div>

                        <div style={{ marginBottom: 10 }}>
                            <Label htmlFor="treview" style={{ width: 250 }}>Τι βαθμο βαζετε στον καθηγητη;</Label>
                            <input value={this.state.selectRating} onChange={this.handleChangeRating} style={{ width: 50 }}></input>
                            <div className="error">{this.getError('review')}</div>
                        </div>

                        <div style={{ marginBottom: 10 }}>
                            <Label htmlFor="tcomments" align="top">Σχολια:</Label>
                            <textarea value={this.state.selectComments} onChange={this.handleChangeComments} style={{ width: '100%', height: 120 }}></textarea>
                        </div>

                    </Form>
                </div>
                <div className="dialog-button">
                    <LinkButton style={{ width: 80 }} onClick={() => this.submitReview()}>Save</LinkButton>
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
                <DataGrid data={this.state.reviews} style={{ height: 550, padding: '15' }}>
                    <GridColumn field="id" title="PrID" hidden="true"></GridColumn>
                    <GridColumn field="fullName" title="Name" align="center"></GridColumn>
                    <GridColumn field="mail" title="Mail" align="center"></GridColumn>
                    <GridColumn field="department" title="Department" align="center"></GridColumn>
                    <GridColumn field="averageRating" title="Average rating" align="center"></GridColumn>
                </DataGrid>
                {this.renderDialog()}
                <LinkButton style={{ width: '100%' }} onClick={() => this.addReview()}>Add your review</LinkButton>
            </div>
        );
    }
}

export default Home;
