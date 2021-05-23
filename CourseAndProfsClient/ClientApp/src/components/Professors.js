import React, { Component, useState } from 'react';
import { DataGrid, GridColumn, Form, Dialog, TextBox, NumberBox, Label, LinkButton, ComboBox, ButtonGroup } from 'rc-easyui';
import axios from 'axios';
export class Professors extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            dataProfs: this.getProfessors(),
            professors: []
        }
    }
    getProfessors() {
        axios.get(`https://${window.location.host}/api/professor?itemsPerPage=20&page=1`)
            .then(res => {
                const professors = res.data.results;
                this.setState({ professors });
            })
    }
    render() {
        return (
            <div>
                <h2>Professors</h2>
                <DataGrid data={this.state.professors} style={{ height: 550, padding: '15' }}>
                    <GridColumn field="id" title="PrID" hidden="true"></GridColumn>
                    <GridColumn field="fullName" title="Name" align="center" ></GridColumn>
                    <GridColumn field="mail" title="Mail" align="center"></GridColumn>
                    <GridColumn field="phone" title="Phone" align="center"></GridColumn>
                    <GridColumn field="office" title="Office" align="center"></GridColumn>
                    <GridColumn field="eOffice" title="E-Office" align="center"></GridColumn>
                    <GridColumn field="department" title="Department" align="center"></GridColumn>
                    <GridColumn field="averageRating" title="Average rating" align="center"></GridColumn>
                </DataGrid>
            </div>
        );
    }
}
export default Professors;