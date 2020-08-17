import React, { Component } from 'react';
import * as signalR from "@microsoft/signalr";
import Moment from 'react-moment';


export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.state = {
            messages: [],
            hubConnection: null,
            colorLabel: []
        };
    }

    GetColor = (id) => {
        var valor = this.state.colorLabel.filter(item => item.id == id);

        if (valor.length == 0) {

            var letters = '0123456789ABCDEF';
            var valor = '#';
            for (var i = 0; i < 6; i++) {
                valor += letters[Math.floor(Math.random() * 16)];
            }
            this.setState({ colorLabel: [{ id, valor }, ...this.state.colorLabel] });
            return valor;
        }
        else {
            return valor[0].valor;
        }

    }

    componentDidMount = () => {
        const hubConnection = new signalR.HubConnectionBuilder()
            .withUrl("/kafka")
            .configureLogging(signalR.LogLevel.Information)
            .build();

        this.setState({ hubConnection }, () => {
            this.state.hubConnection
                .start()
                .then(() => console.log('Connection started!'))
                .catch(err => console.log('Error while establishing connection :('));

            this.state.hubConnection.on('kafkaService', (message) => {
                console.info(message);

                this.setState({ messages: [message, ...this.state.messages] });
            });
        });
    }

    render() {

        return (
            <div>
                <h1>Customer Kafka</h1>

                <div>
                    <table className='table table-striped' aria-labelledby="tabelLabel">
                        <thead>
                            <tr>
                                <th>Date</th>
                                <th>RequestId</th>
                                <th>Instance</th>
                                <th>Message</th>
                            </tr>
                        </thead>
                        <tbody>
                            {this.state.messages.map(item => {
                                return (<tr key={item.requestId}>
                                    <td><Moment format="HH:mm:ss.SSS">{item.createdAt}</Moment></td>
                                    <td>{item.requestId}</td>
                                    <td><span style={{ background: this.GetColor(item.instanceId), color: '#fff', padding: '5px', border_radius: '5px' }}>{item.instanceId}</span></td>
                                    <td>{item.message}</td>
                                </tr>);
                            }
                            )}
                        </tbody>
                    </table>
                </div>
            </div>
        );
    }
}
