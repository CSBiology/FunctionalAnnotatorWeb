var DataAnnotatorApp = function () {

    var DataAnnotatorService = function (baseUri) {

        var _baseUri = baseUri;

        var createHttpError = function (xhttp) {
            return new Error("Http Error - Status code: " + xhttp.status + ", Status text: " + xhttp.statusText);
        };

        this.searchFunctionsAsync = function (ontologyId, organismId, searchName) {

            var url = _baseUri + '/DataAnnotator/SearchFunctions';
            var xhttp = new XMLHttpRequest();
            var deferred = Q.defer();

            xhttp.onerror = function () {
                deferred.reject(new Error("Can't XHR " + JSON.stringify(url)));
            };

            xhttp.onload = function () {
                if (this.status == 200) {
                    var result = JSON.parse(this.responseText);
                    deferred.resolve(result);
                }
                else {
                    deferred.reject(createHttpError(xhttp));
                }
            };

            var query = { 'OntologyId': ontologyId, 'OrganismId': organismId, 'SearchName': searchName };

            xhttp.open("POST", url, true);
            xhttp.setRequestHeader("Content-type", "application/json");
            xhttp.send(JSON.stringify(query));

            return deferred.promise;
        };

        this.getOntologiesAsync = function () {

            var url = _baseUri + '/DataAnnotator/GetOntologies';
            var xhttp = new XMLHttpRequest();
            var deferred = Q.defer();

            xhttp.onerror = function () {
                deferred.reject(new Error("Can't XHR " + JSON.stringify(url)));
            };

            xhttp.onload = function () {
                if (this.status == 200) {
                    var result = JSON.parse(this.responseText);
                    deferred.resolve(result);
                }
                else {
                    deferred.reject(createHttpError(xhttp));
                }
            };

            xhttp.open("GET", url, true);
            xhttp.send();

            return deferred.promise;
        };

        this.getSourceOrganismsAsync = function () {

            var url = _baseUri + '/DataAnnotator/GetSourceOrganisms';
            var xhttp = new XMLHttpRequest();
            var deferred = Q.defer();

            xhttp.onerror = function () {
                deferred.reject(new Error("Can't XHR " + JSON.stringify(url)));
            };

            xhttp.onload = function () {
                if (this.status == 200) {
                    var result = JSON.parse(this.responseText);
                    deferred.resolve(result);
                }
                else {
                    deferred.reject(createHttpError(xhttp));
                }
            };

            xhttp.open("GET", url, true);
            xhttp.send();

            return deferred.promise;
        };
    };

    var DataSetRecord = function (data) {

        if (Array.isArray(data) == false)
            throw new TypeError('Array type expected.');

        var _self = this;
        var _data = data;
        var _functionTerms = [];

        Object.defineProperty(_self, 'data', {
            get: function () { return _data; }
        });

        Object.defineProperty(_self, 'hasTerms', {
            get: function () {
                return _functionTerms != null && _functionTerms.length > 0;
            }
        });

        Object.defineProperty(_self, 'functionTerms', {
            get: function () { return _functionTerms; },
            set: function (value) {
                if (Array.isArray(value) == false)
                    throw new TypeError('Array type expected.');
                else
                    _functionTerms = value;
            }
        });

        this.stringify = function () {

            var termNames = _functionTerms
                .map(function (term) { return term.name })
                .join(';');

            var values = _data.join('\t');

            return termNames.concat('\t').concat(values);
        };
    };

    var DataSet = function (name) {

        var _self = this;
        var _columnNames = [];
        var _records = [];
        var _name = name;

        var normalizeRecord = function (record, expectedLength) {

            var normalized = record.slice(0);

            if (normalized.length != expectedLength) {

                normalized.length = expectedLength;

                for (var i = record.length; i < expectedLength; i++)
                    normalized[i] = '';
            }

            return normalized;
        }

        Object.defineProperty(_self, 'name', {
            get: function () { return _name; }
        });

        Object.defineProperty(_self, 'columnNames', {
            get: function () { return _columnNames; },
            set: function (value) { _columnNames = value; }
        });

        Object.defineProperty(_self, 'records', {
            get: function () { return _records; }
        });

        this.loadFromFileAsync = function (file) {

            if (file == null)
                throw new Error('Argument error: file cannot be null.');

            _columnNames = [];
            _records = [];

            var reader = new FileReader();
            var deferred = Q.defer();

            reader.onerror = function (evt) {
                deferred.reject(new Error('Error reading data.'));
            };

            reader.onloadend = function (evt) {

                var lines = evt.target.result.split(/\r\n|\n/);

                // create col names

                if (lines.length >= 1) {

                    var values = lines[0].split(/\t/);

                    if (values.length == 0)
                        Q.reject(new Error('Column names cannot be empty.'));

                    for (var i = 0; i < values.length; i++) {

                        var colName = values[i].trim();

                        if (colName.length == 0) {
                            colName = 'Column_'.concat(i);
                        }

                        _columnNames.push(colName);
                    }
                }

                // create records

                var recCount = 0;

                for (var i = 1; i < lines.length; i++) {
                    if (lines[i].trim().length > 0) { // ignore empty lines
                        var data = lines[i].split(/\t/);
                        var normalized = normalizeRecord(data, _columnNames.length);
                        _records.push(new DataSetRecord(normalized));
                        recCount++;
                    }
                }

                // notify async resolved
                // return number of records read

                deferred.resolve(recCount);
            };

            reader.readAsText(file);

            return deferred.promise;
        }

        this.stringify = function () {

            var columns = ["Function Terms()"]
                .concat(_columnNames)
                .join('\t');

            var records = _records
                .map(function (record) {
                    return record.stringify();
                }).join('\n');

            return columns.concat('\n').concat(records);
        };
    }

    var OntologyDataSource = function (annotatorService) {

        var _self = this;
        var _svc = annotatorService;
        var _ontologies = [];

        Object.defineProperty(_self, 'ontologies', {
            get: function () { return _ontologies; }
        });

        this.loadOntologiesAsync = function () {
            return _svc.getOntologiesAsync()
                .then(function (ontologies) {
                    _ontologies = ontologies;
                    return ontologies;
                });
        };
    };

    var SourceOrganismsDataSource = function (annotatorService) {

        var _self = this;
        var _svc = annotatorService;
        var _sourceOrganisms = [];

        Object.defineProperty(_self, 'sourceOrganisms', {
            get: function () { return _sourceOrganisms; }
        });

        this.loadSourceOrganismsAsync = function () {
            return _svc.getSourceOrganismsAsync()
                .then(function (sourceOrganisms) {
                    _sourceOrganisms = sourceOrganisms;
                    return sourceOrganisms;
                });
        };
    };

    var DataSetProcessing = function (annotatorService) {

        var _self = this;
        var _svc = annotatorService;
        var _processingAborted = false;

        Object.defineProperty(_self, 'processingAborted', {
            get: function () { return _processingAborted; }
        });

        this.processDataSetAsync = function (dataSet, queryColumnIndex, ontologyId, organismId) {

            if (dataSet == null)
                return Q.reject(new Error('Argument error: dataSet cannot be null.'));
            if (isNaN(queryColumnIndex))
                return Q.reject(new TypeError('Argument queryColumnIndex is not a number.'));
            if (queryColumnIndex < 0 || queryColumnIndex >= dataSet.columnNames.length)
                return Q.reject(new Error('Argument error: queryColumnIndex out of range.'));
            if (ontologyId == null || ontologyId.trim().length <= 0)
                return Q.reject(new Error('Argument error: ontologyId may not be empty.'));
            if (organismId == null || organismId.trim().length <= 0)
                return Q.reject(new Error('Argument error: organismId may not be empty.'));

            var deferred = Q.defer();

            _processingAborted = false;

            var recCount = dataSet.records.length;

            if (recCount == 0)
                return deferred.resolve();

            var processRecordAsync = function (recIdx) {

                if (recIdx >= recCount || _processingAborted) {
                    return recIdx;
                }
                else {

                    var record = dataSet.records[recIdx];
                    var searchName = record.data[queryColumnIndex].trim();

                    if (searchName.length > 0) {

                        deferred.notify('Searching ' + searchName);

                        return _svc.searchFunctionsAsync(ontologyId, organismId, searchName)
                            .then(function (functions) {
                                record.functionTerms = functions;
                                return recIdx + 1;
                            })
                            .then(processRecordAsync)
                            .catch(deferred.reject);
                    }
                    else {
                        deferred.notify('Search name is empty, skip.');
                        record.functionTerms = [];
                        return processRecordAsync(recIdx + 1);
                    }
                }
            }

            Q(0).then(processRecordAsync)
                .then(deferred.resolve)
                .catch(deferred.reject);

            return deferred.promise;
        }

        this.abort = function () {
            _processingAborted = true;
        }
    };

    var _svc = new DataAnnotatorService('http://' + document.location.host);
    var _dataSet = null;
    var _ontologies = new OntologyDataSource(_svc);
    var _sourceOrganisms = new SourceOrganismsDataSource(_svc);
    var _processing = new DataSetProcessing(_svc);
    var _selectedOntologyId = null;
    var _selectedOrganismId = null;
    var _selectedColumnIndex = null;

    var _selectFileButton = document.getElementById('selectFileButton');
    var _runButton = document.getElementById('runButton');
    var _stopButton = document.getElementById('stopButton');
    var _exportResultsButton = document.getElementById('exportResultsButton');
    var _selectOntology = document.getElementById('selectOntology');
    var _selectOrganism = document.getElementById('selectOrganism');
    var _selectColumn = document.getElementById('selectColumn');
    var _selectFile = document.getElementById('selectFile');
    var _fileName = document.getElementById('fileName');
    var _glasspane = document.getElementById('glasspane');
    var _messages = document.getElementById('messages');
    var _dataTable = document.getElementById('dataTable');

    var clearMessages = function () {
        _messages.value = '';
    };

    var logMessage = function (message) {
        _messages.value += message + '\n';
    };

    var logError = function (err) {
        logMessage('ERROR: ' + err.message);
    };

    var setGlasspane = function (visible) {

        if (visible) {
            _glasspane.style.visibility = "visible";
        }
        else {
            _glasspane.style.visibility = "hidden";
        }
    };

    var getSelectedValue = function (select) {
        if (select.selectedIndex >= 0) {
            return select.value;
        }
        else {
            return null;
        }
    }

    var updateSelectOrganism = function () {

        _selectOrganism.innerHTML = '';

        for (var i = 0; i < _sourceOrganisms.sourceOrganisms.length; i++) {
            var opt = document.createElement('option');
            opt.value = _sourceOrganisms.sourceOrganisms[i].id;
            opt.text = _sourceOrganisms.sourceOrganisms[i].name;
            _selectOrganism.appendChild(opt);
        }

        _selectedOrganismId = getSelectedValue(_selectOrganism);
    }

    var updateSelectOntology = function () {

        _selectOntology.innerHTML = '';

        for (var i = 0; i < _ontologies.ontologies.length; i++) {
            var opt = document.createElement('option');
            opt.value = _ontologies.ontologies[i].id;
            opt.text = _ontologies.ontologies[i].name;
            _selectOntology.appendChild(opt);
        }

        _selectedOntologyId = getSelectedValue(_selectOntology);
    }

    var updateSelectColumn = function () {

        _selectColumn.innerHTML = '';

        for (var i = 0; i < _dataSet.columnNames.length; i++) {
            var opt = document.createElement('option');
            opt.value = i;
            opt.text = _dataSet.columnNames[i];
            _selectColumn.appendChild(opt);
        }

        _selectedColumnIndex = getSelectedValue(_selectColumn);
    }

    var createDataTableColumns = function () {

        var thead = document.createElement('thead');
        var tr = document.createElement('tr');

        thead.appendChild(tr);

        var createColumn = function (tr, name) {
            var td = document.createElement('td');
            td.style.whiteSpace = 'nowrap';            
            var strong = document.createElement('strong');
            strong.textContent = name;
            td.appendChild(strong);
            tr.appendChild(td);
        }

        createColumn(tr, 'Function Term(s)');

        for (var col = 0; col < _dataSet.columnNames.length; col++) {
            createColumn(tr, _dataSet.columnNames[col]);
        }

        return thead;
    }

    var createDataTableRows = function () {

        var tbody = document.createElement('tbody');

        var createTermCell = function (record) {

            var td = document.createElement('td');

            if (record.hasTerms) {

                var select = document.createElement('select');
                select.style.width = '100%';
                select.style.border = 'none';
                select.style.backgroundColor = 'transparent';

                for (var i = 0; i < record.functionTerms.length; i++) {
                    var opt = document.createElement('option');
                    opt.value = record.functionTerms[i].id;
                    opt.text = record.functionTerms[i].name;
                    select.appendChild(opt);
                }

                td.appendChild(select);
            }
            else {
                td.textContent = '';
            }

            return td;
        }

        for (var row = 0; row < _dataSet.records.length; row++) {

            var tr = document.createElement('tr');
            var record = _dataSet.records[row];

            tr.appendChild(createTermCell(record));

            var recData = record.data;

            for (var col = 0; col < recData.length; col++) {
                var td = document.createElement('td');
                td.textContent = recData[col];
                tr.appendChild(td);
            }

            tbody.appendChild(tr);

        }

        return tbody;
    }

    var updateDataTable = function () {
        _dataTable.innerHTML = '';
        var thead = createDataTableColumns();
        var tbody = createDataTableRows();
        _dataTable.appendChild(thead);
        _dataTable.appendChild(tbody);
    }

    var updateFileName = function () {
        _fileName.value = _dataSet.name;
    }

    var ontologySelectionChange = function (event) {
        _selectedOntologyId = getSelectedValue(event.target);
    }

    var organismSelectionChange = function (event) {
        _selectedOrganismId = getSelectedValue(event.target);
    }

    var columnSelectionChange = function (event) {
        _selectedColumnIndex = getSelectedValue(event.target);
    }

    var selectFileButtonClick = function () {

        var selectFileCompleted = function () {
            setGlasspane(false);
            logMessage('Loading file completed.');
            updateFileName();
            updateSelectOntology();
            updateSelectOrganism();
            updateSelectColumn();
            updateDataTable();
        }

        var selectFileFailed = function (err) {
            logError(err);
            setGlasspane(false);
            alert('Error loading file. See log messages for details.');
        }

        var fileSelectionChange = function (event) {

            if (event.target.files.length > 0) {
                var file = event.target.files[0];

                if (file == null) {
                    return;
                }

                if (file.size > 262144000) {
                    alert('Files greater than 250 Mb not supported.');
                    return;
                }

                if (file.type != 'text/plain') {
                    alert('Wrong file type, only text files allowed.');
                    return;
                };

                setGlasspane(true);
                clearMessages();

                logMessage('Loading file: ' + file.name);

                _dataSet = new DataSet(file.name);

                _ontologies.loadOntologiesAsync()
                    .then(_sourceOrganisms.loadSourceOrganismsAsync)
                    .thenResolve(file)
                    .then(_dataSet.loadFromFileAsync)
                    .then(selectFileCompleted)
                    .catch(selectFileFailed);
            }
        };

        clearMessages();

        _selectFile.onchange = function () { };
        _selectFile.value = null;
        _selectFile.onchange = fileSelectionChange;
        _selectFile.click();
    };

    var runButtonClick = function () {

        if (_dataSet == null) {
            alert('No Dataset Selected.');
            return;
        };

        if (_selectedOntologyId == null) {
            alert('No ontology selected.');
            return;
        }

        if (_selectedOrganismId == null) {
            alert('No source organism selected.');
            return;
        }

        if (_selectedColumnIndex == null) {
            alert('No column selected.');
            return;
        }

        clearMessages();
        setGlasspane(true);

        var completed = function (numRecs) {

            updateDataTable();

            if (_processing.processingAborted) {
                logMessage('Processing dataset cancelled.');
            }
            else {
                logMessage('Processing dataset completed. Number of records: ' + numRecs);
            }

            setGlasspane(false);
        }

        var progress = function (msg) {
            logMessage(msg);
        }

        var error = function (err) {
            logError(err);
            setGlasspane(false);
            alert('Error processing dataset. See log messages for details.');
        }

        logMessage('Processing dataset: ' + _dataSet.name);

        _processing.processDataSetAsync(_dataSet, _selectedColumnIndex, _selectedOntologyId, _selectedOrganismId)
            .then(completed, error, progress);
    };

    var stopButtonClick = function () {
        _processing.abort();
    };

    var exportButtonClick = function () {

        var isFileSaverSupported = false;

        try {
            isFileSaverSupported = !!new Blob;
        } catch (e) { }

        if (isFileSaverSupported == false) {
            alert('Blob API not supported by your browser.');
            return;
        }

        if (_dataSet == null) {
            alert('No Dataset Selected.');
            return;
        };

        var data = _dataSet.stringify();
        var blob = new Blob([data], { type: "text/plain;charset=utf-8" });
        saveAs(blob, _dataSet.name + "_results.txt");

    };

    this.run = function () {
        _selectFileButton.onclick = selectFileButtonClick;
        _stopButton.onclick = stopButtonClick;
        _runButton.onclick = runButtonClick;
        _exportResultsButton.onclick = exportButtonClick;
        _selectOntology.onchange = ontologySelectionChange;
        _selectColumn.onchange = columnSelectionChange;
        _selectOrganism.onchange = organismSelectionChange;
    };

};